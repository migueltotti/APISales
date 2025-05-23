using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sales.Infrastructure.ModelsConfiguration;

public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        var userRole = new IdentityUserRole<string>();
        userRole.UserId = "4de2810f-b57a-4aa0-b364-057c809160f9";
        userRole.RoleId = "a5d55a1d-a654-452d-a24a-6f69985c11e3";

        builder.HasData(userRole);
    }
}