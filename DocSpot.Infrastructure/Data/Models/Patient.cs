namespace DocSpot.Infrastructure.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using DocSpot.Infrastructure.Data.Abstracts;
    using Microsoft.AspNetCore.Identity;

    public class Patient :  BaseModel
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public required string UserId { get; set; } = string.Empty;

        // Navligation properties

        public List<Appointment> Appointments { get; set; } = new List<Appointment>();

        public IdentityUser User { get; set; }
    }
}
