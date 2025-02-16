namespace DocSpot.Infrastructure.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Microsoft.AspNetCore.Identity;

    using DocSpot.Infrastructure.Data.Abstracts;

    public class Doctor : BaseModel
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public required string UserId { get; set; } = string.Empty;

        // Navigation properties

        public List<Schedule> Schedules { get; set; } = new List<Schedule>();

        public IdentityUser User { get; set; } = null!;
    }
}
