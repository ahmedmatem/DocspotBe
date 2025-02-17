namespace DocSpot.Infrastructure.Data.Configurations
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class IdentityUserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.HasData(SeedAdminUserInRoleAdmin());
        }

        private IdentityUserRole<string> SeedAdminUserInRoleAdmin()
        {
            return new IdentityUserRole<string>()
            {
                UserId = Constants.AdminUserId,
                RoleId = Constants.RoleAdminId
            };
        }
    }
}
