using DDD.OrdersApp.API.Filters;
using DDD.OrdersApp.API.Middleware;
using DDD.OrdersApp.Application.Auth.DTOs;
using DDD.OrdersApp.Application.Auth.Handlers.Commands;
using DDD.OrdersApp.Application.Common;
using DDD.OrdersApp.Application.Orders.DTOs;
using DDD.OrdersApp.Application.Orders.Handlers.Commands;
using DDD.OrdersApp.Application.Orders.Handlers.Queries;
using DDD.OrdersApp.Infrastructure.Cache;
using DDD.OrdersApp.Infrastructure.Kafka;
using DDD.OrdersApp.Infrastructure.Orders.Data;
using DDD.OrdersApp.Infrastructure.Orders.Repositories;
using FluentValidation;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Reflection;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());



Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddHostedService(sp =>
    new OutboxPublisherService(
        sp,
        builder.Configuration["Kafka:BootstrapServers"],
        builder.Configuration["Kafka:Topic"],
        sp.GetRequiredService<ILogger<OutboxPublisherService>>()
    ));

builder.Host.UseSerilog();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddScoped(typeof(ValidationFilter<>));
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("Postgres")));
builder.Services.AddSingleton(x => new RedisCacheService(configuration["Redis:ConnectionString"]));
builder.Services.AddSingleton(x => new KafkaProducerService(configuration["Kafka:BootstrapServers"]));
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IHandler<CreateOrderCommand, Result<OrderDto>>>(sp =>
    new CreateOrderCommandHandler(
        sp.GetRequiredService<IOrderRepository>(),
        sp.GetRequiredService<RedisCacheService>(),
        sp.GetRequiredService<KafkaProducerService>(),
        builder.Configuration["Kafka:Topic"]
    ));

builder.Services.AddScoped<IHandler<GetOrderQuery, Result<OrderDto>>>(sp =>
    new GetOrderQueryHandler(
        sp.GetRequiredService<IOrderRepository>(),
        sp.GetRequiredService<RedisCacheService>()
    ));
builder.Services.AddScoped<IHandler<RegisterUserDto, Result<bool>>>(sp =>
    new RegisterUserCommandHandler(
        sp.GetRequiredService<IUserRepository>()
    ));

builder.Services.AddScoped<IHandler<LoginRequestDto, Result<LoginResponse>>>(sp =>
    new LoginCommandHandler(
        sp.GetRequiredService<IUserRepository>(),
        sp.GetRequiredService<IConfiguration>()
    ));


var kafkaProducerConfig = new Confluent.Kafka.ProducerConfig
{
    BootstrapServers = builder.Configuration["Kafka:BootstrapServers"],
    SocketTimeoutMs = 2000,
    MessageTimeoutMs = 2000,
    RequestTimeoutMs = 2000
};

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Postgres")!, name: "postgres")
    .AddRedis(builder.Configuration["Redis:ConnectionString"]!, name: "redis")
    .AddKafka(kafkaProducerConfig, name: "kafka");

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapHealthChecks("/health2", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
});
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                error = e.Value.Exception?.Message
            }),
            totalDuration = report.TotalDuration.TotalMilliseconds
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(result));
    }
});
app.MapControllers();

app.Run();

