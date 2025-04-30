namespace Sales.Domain.Interfaces;

public interface IEncryptService
{
    string Encrypt(string value);
    string Decrypt(string value);
}