namespace DocSpot.Infrastructure.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using DocSpot.Infrastructure.Data.Abstracts;

    public class Doctor : BaseModel
    {
        [Required]
        public required string Name { get; set; }

        // Navigation properties

        public List<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}
