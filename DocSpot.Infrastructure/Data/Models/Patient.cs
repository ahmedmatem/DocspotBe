namespace DocSpot.Infrastructure.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using DocSpot.Infrastructure.Data.Abstracts;

    public class Patient :  BaseModel
    {
        [Required]
        public required string Name { get; set; }

        // Navligation properties

        public List<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
