
using DDD.OrdersApp.Domain.Interfaces;
namespace DDD.OrdersApp.Infrastructure.Encription;

public class BcryptEncryption : IEncryption
{
    public string Encrypt(string plainText) =>  BCrypt.Net.BCrypt.HashPassword(plainText);


    public bool VerifyEncryption( string plaintext, string encryptedText)
        => BCrypt.Net.BCrypt.Verify(plaintext, encryptedText);

}
