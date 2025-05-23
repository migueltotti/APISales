using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sales.Infrastructure.Identity;

namespace Sales.Infrastructure.ModelsConfiguration;

public class UserLoginConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        var user = new ApplicationUser();
        user.Id = "4de2810f-b57a-4aa0-b364-057c809160f9";
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.Now;
        user.UserName = "Admin-dmaamm";
        user.NormalizedUserName = "ADMIN-DMAAMM";
        user.Email = "admin@gmail.com";
        user.NormalizedEmail = "ADMIN@GMAIL.COM";
        user.EmailConfirmed = false;
        user.PasswordHash = "AQAAAAIAAYagAAAAEEyRyv+ur5EUUt/0XE1Ptn12KryCQTpV1UEtn6sghOSd7bvnKIEPGUv94bMFvsIVeg==";
        user.ConcurrencyStamp = "19947c07-0772-45e5-8bce-7014b9ad8ac3";
        user.PhoneNumber = null;
        user.PhoneNumberConfirmed = false;
        user.TwoFactorEnabled = false;
        user.LockoutEnd = null;
        user.LockoutEnabled = true;
        user.AccessFailedCount = 0;

        builder.HasData(user);
    }
}