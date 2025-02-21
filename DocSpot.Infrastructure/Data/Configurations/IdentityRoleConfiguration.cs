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
                    Name = Role.Admin,
                    NormalizedName = Role.Admin,
                    ConcurrencyStamp = Constants.RoleAdminId,
                },
                new IdentityRole()
                {
                    Id = Constants.RoleDoctorId,
                    Name = Role.Doctor,
                    NormalizedName = Role.Doctor.ToUpper(),
                    ConcurrencyStamp = Constants.RoleDoctorId,
                },
                new IdentityRole()
                {
                    Id = Constants.RolePatientId,
                    Name = Role.Patient,
                    NormalizedName = Role.Patient.ToUpper(),
                    ConcurrencyStamp = Constants.RolePatientId,
                },
            ];
        }
    }
}
