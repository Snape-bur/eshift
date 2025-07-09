using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; 

namespace EShift.Models
{
    // Enum for the operational status or availability of a driver
    public enum DriverStatus
    {
        Available,    // Driver is ready and available for new jobs
        OnJob,        // Driver is currently assigned to and working on a job
        OffDuty,      // Driver is not working (e.g., end of shift, weekend)
        OnBreak,      // Driver is on a short break
        SickLeave,    // Driver is unavailable due to sickness
    
    }

    public class Driver
    {
        [Key] // Marks DriverId as the primary key
        [Required] // Ensures the ID is always present
        public int DriverId { get; set; } // Renamed from 'Id' for consistency with VehicleId and JobId

        [Required] // Ensures FullName is always present
        public string FullName { get; set; }

        [Required] // Ensures a status is always set for the driver
        public DriverStatus Status { get; set; } // Using enum for clear, predefined driver states

        [Required] // Ensures PhoneNumber is always present
        public string PhoneNumber { get; set; }

        public string? LicenseNumber { get; set; } // Made nullable as it might not be immediately available or universally required

        
        public ICollection<Job>? AssignedJobs { get; set; }
    }
}