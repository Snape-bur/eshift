using System.ComponentModel.DataAnnotations;

namespace EShift.Models
{
    public class Driver
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string LicenseNumber { get; set; }

        public string PhoneNumber { get; set; }

        // Optional: Navigation property
        public ICollection<TransportUnit>? TransportUnits { get; set; }
    }
}
