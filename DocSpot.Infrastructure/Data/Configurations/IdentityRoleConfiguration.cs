#nullable disable
namespace DocSpot.Infrastructure.Data.Configurations
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using DocSpot.Infrastructure.Data.Types;

    public class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(SeedRoles());
        }

        private IdentityRole[] SeedRoles()
        {
            return [ 
                new IdentityRole()
                {
                    Id = "59e7a7f0-84d7-4d5a-a59a-0d7eff4509c8",
                    Name = Role.Admin.ToString(),
                    NormalizedName = Role.Admin.ToString().ToUpper(),
                    ConcurrencyStamp = "59e7a7f0-84d7-4d5a-a59a-0d7eff4509c8",
                },
                new IdentityRole()
                {
                    Id = "6455d797-d71d-4587-963d-0c7c4ac69420",
                    Name = Role.Doctor.ToString(),
                    NormalizedName = Role.Doctor.ToString().ToUpper(),
                    ConcurrencyStamp = "6455d797-d71d-4587-963d-0c7c4ac69420",
                },
                new IdentityRole()
                {
                    Id = "f64eeef7-bf35-4b8d-9db6-ace187b9f20e",
                    Name = Role.Patient.ToString(),
                    NormalizedName = Role.Patient.ToString().ToUpper(),
                    ConcurrencyStamp = "f64eeef7-bf35-4b8d-9db6-ace187b9f20e",
                },
            ];
        }
    }
}
