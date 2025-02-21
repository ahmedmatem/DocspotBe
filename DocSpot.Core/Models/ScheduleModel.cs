namespace DocSpot.Core.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.EntityFrameworkCore;

    using DocSpot.Infrastructure.Data.Models;

    /// <summary>
    /// "date": "2024-02-15",
    /// "startTime": "08:00:00",
    /// "endTime": "16:00:00"
    /// </summary>
    public class ScheduleModel
    {
        /// <summary>
        /// The date of the schedule
        /// </summary>
        [Required]
        [Display(Name = "Дата")]
        public required string Date { get; set; }

        [Required]
        [Display(Name = "Начало")]
        public required string StartTime { get; set; }

        [Required]
        [Display(Name = "Край")]
        public required string EndTime { get; set; }

        /// <summary>
        /// Unique identifier of the doctor
        /// </summary>
        public required string DoctorId { get; set; }
    }
}
