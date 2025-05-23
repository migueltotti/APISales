using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sales.Infrastructure.ModelsConfiguration;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        var admin = new IdentityRole("Admin");
        admin.Id = "a5d55a1d-a654-452d-a24a-6f69985c11e3";
        admin.NormalizedName = admin.Name.ToUpper();
        admin.ConcurrencyStamp = Guid.NewGuid().ToString();
        
        var employee = new IdentityRole("Employee");
        employee.Id = "b890f0aa-7486-4aa9-ba41-c76609a76476";
        employee.NormalizedName = employee.Name.ToUpper();
        employee.ConcurrencyStamp = Guid.NewGuid().ToString();
        
        var customer = new IdentityRole("Customer");
        customer.Id = "8a19b5bc-91ed-4399-8953-046eb2e1de37";
        customer.NormalizedName = customer.Name.ToUpper();
        customer.ConcurrencyStamp = Guid.NewGuid().ToString();
        
        builder.HasData(
            admin,
            employee,
            customer
        );
    }
}