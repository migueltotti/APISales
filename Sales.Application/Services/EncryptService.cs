using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Sales.Application.Interfaces;

namespace Sales.Application.Services;

public class EncryptService : IEncryptService
{
    private readonly string _publicKey;
    private readonly string _privateKey;
    private readonly IConfiguration _configuration;

    public EncryptService(IConfiguration configuration)
    {
        _configuration = configuration;
        _privateKey = _configuration["PRIVATE_KEY"] ?? throw new ArgumentNullException("OpenSSL null private key");
        _publicKey = _configuration["PUBLIC_KEY"] ?? throw new ArgumentNullException("OpenSSL null public key");
    }

    public string Encrypt(string value)
    {
        var valueInBytes = Encoding.UTF8.GetBytes(value);

        using var rsa = new RSACryptoServiceProvider(1024);
        try
        {
            // set PEM public key to rsa algorithm
            rsa.ImportFromPem(_publicKey);
                
            // encrypt the value in bytes
            var encryptedValue = rsa.Encrypt(valueInBytes, false);
                
            // convert to base64
            var encrypetValueIn64 = Convert.ToBase64String(encryptedValue);

            return encrypetValueIn64;
        }
        finally
        {
            rsa.PersistKeyInCsp = false;
        }
    }

    public string Decrypt(string value)
    {
        using var rsa = new RSACryptoServiceProvider(1024);
        try
        {
            // convert base 64 to base 8
            var encryptedValueInBytes = Convert.FromBase64String(value);
            
            // set PEM private key to rsa algorithm
            rsa.ImportFromPem(_privateKey);
                
            // encrypt the value in bytes
            var decryptedValue = rsa.Decrypt(encryptedValueInBytes, false);
                
            // convert to base64
            var decryptedStringValue = Encoding.UTF8.GetString(decryptedValue);

            return decryptedStringValue;
        }
        finally
        {
            rsa.PersistKeyInCsp = false;
        }
    }
}