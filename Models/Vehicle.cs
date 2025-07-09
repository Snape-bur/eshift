using System.Collections.Generic; // Added for ICollection (for Job relationships)
using System.ComponentModel.DataAnnotations; // Added for [Required]

namespace EShift.Models
{
    // Enum for different types of vehicles, ensuring consistency
    public enum VehicleType
    {
       
     // Added as per your example "Pickup truck"
        MotorCycle, // Added for your JavaScript validation
        FourDoorSedan, 
        FiveDoorCar,
        PickupTruck,
        MultiPurposeVehicle, 
        SixWheelFusoTruck 

    }

    // Enum for the operational status of a vehicle
    public enum VehicleStatus
    {
        Operational,    // Vehicle is ready and available for jobs
        OnJob,          // Vehicle is currently assigned to and working on a job
        UnderMaintenance, // Vehicle is not available due to maintenance
        BrokenDown,     // Vehicle is not available due to a breakdown
        
    }

    public class Vehicle
    {
        public int VehicleId { get; set; } // Primary Key for the Vehicle

        public VehicleType Type { get; set; } // Uses the VehicleType enum (e.g., PickupTruck)

        [Required] // Make is usually a mandatory field for a vehicle
        public string Make { get; set; } // e.g., "Toyota"

        [Required] // Model is usually a mandatory field for a vehicle
        public string Model { get; set; } // e.g., "Hilux"

        public string? Color { get; set; } // e.g., "Grey" - made nullable as color might be optional or not always recorded

        // Capacity of the vehicle, specified in cubic feet for clear measurement
        // Note: Your JavaScript still refers to "kg". Ensure consistency here in your system logic.
        public decimal CapacityCubicFeet { get; set; }

        // Current status of the vehicle, using the VehicleStatus enum
        public VehicleStatus Status { get; set; }

        public string LicensePlate { get; set; } // Unique identifier for the vehicle

       
        public ICollection<Job>? AssignedJobs { get; set; }
    }
}