namespace Sales.Domain.Interfaces;

public interface ICacheService
{
    public Task RemoveAsync(string key);
}