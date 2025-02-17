namespace DocSpot.Infrastructure.Data.Configurations
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class IdentityUserConfiguration : IEntityTypeConfiguration<IdentityUser>
    {
        public void Configure(EntityTypeBuilder<IdentityUser> builder)
        {
            builder.HasData(SeedAdminUser());
        }

        private IdentityUser SeedAdminUser()
        {
            string adminUserId = Constants.AdminUserId;
            string adminEmail = Constants.AdminEmail;
            string adminPassword = Constants.AdminPassword;

            var adminUser = new IdentityUser
            {
                Id = adminUserId,
                UserName = adminEmail,
                NormalizedUserName = adminEmail.ToString(),
                Email = adminEmail,
                NormalizedEmail = adminEmail.ToString(),
                EmailConfirmed = true,
                SecurityStamp = adminUserId
            };

            // Hash the password
            PasswordHasher<IdentityUser> passwordHasher = new PasswordHasher<IdentityUser>();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, adminPassword);

            return adminUser;
        }
    }
}
