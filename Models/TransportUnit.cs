using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShift.Models
{
    public class TransportUnit
    {
        [Key]
        public int VehicleId { get; set; }

        [Required]
        [Display(Name = "License Plate")]
        public string LicensePlate { get; set; }

        [Required]
        [Display(Name = "Vehicle Type")]
        public string VehicleType { get; set; }

        [Required]
        public string Make { get; set; }

        [Required]
        public string Model { get; set; }

        [Range(1900, 2100)]
        public int Year { get; set; }

        [Required]
        [Display(Name = "Capacity (kg)")]
        public int Capacity { get; set; }

        [Required]
        public string Status { get; set; }

        // Optional foreign key for Driver
        public int? DriverId { get; set; }

        [ForeignKey("DriverId")]
        public virtual Driver? Driver { get; set; } // Only if Driver table exists
    }
}
