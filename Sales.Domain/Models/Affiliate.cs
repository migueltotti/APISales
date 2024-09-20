namespace Sales.Domain.Models;

public class Affiliate
{
    public int AffiliateId { get; private set; }
    public string? Name { get; private set; }
    public decimal Discount { get; private set; }
    
    // Affiliate 1 : n User
    public ICollection<User> Users { get; private set; }   

    public Affiliate(int affiliateId, string? name, decimal discount)
    {
        AffiliateId = affiliateId;
        Name = name;
        Discount = discount;
    }
    
    public Affiliate(string? name, decimal discount)
    {
        Name = name;
        Discount = discount;
    }
}