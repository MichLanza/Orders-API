
namespace DDD.OrdersApp.Domain.Interfaces;

public  interface IEncryption
{
    public string Encrypt(string plainText);
    public bool VerifyEncryption(string plaintext, string encryptedText);
}
