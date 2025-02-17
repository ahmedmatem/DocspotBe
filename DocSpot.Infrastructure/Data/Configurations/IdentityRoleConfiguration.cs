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
                    Id = Constants.RoleAdminId,
                    Name = Role.Admin.ToString(),
                    NormalizedName = Role.Admin.ToString().ToUpper(),
                    ConcurrencyStamp = Constants.RoleAdminId,
                },
                new IdentityRole()
                {
                    Id = Constants.RoleDoctorId,
                    Name = Role.Doctor.ToString(),
                    NormalizedName = Role.Doctor.ToString().ToUpper(),
                    ConcurrencyStamp = Constants.RoleDoctorId,
                },
                new IdentityRole()
                {
                    Id = Constants.RolePatientId,
                    Name = Role.Patient.ToString(),
                    NormalizedName = Role.Patient.ToString().ToUpper(),
                    ConcurrencyStamp = Constants.RolePatientId,
                },
            ];
        }
    }
}
