// EShift.Models/Job.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EShift.Models
{
    public class Job
    {
        public int JobId { get; set; }
        public string AppUserId { get; set; } // Links to AppUser (same as Customer.AppUserId)
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string PickupAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime MoveDate { get; set; }
        public string MoveTime { get; set; }
        public string Category { get; set; }
        public int ItemCount { get; set; }
        public double EstimatedKg { get; set; }

        public string Instructions { get; set; }
        public string Status { get; set; } = "Pending"; // e.g., Pending, Approved, In Progress, Completed, Rejected, Canceled
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Initialize on creation
        public DateTime EstimatedArrival { get; set; } // New property for assignment
        public int ProgressPercentage { get; set; } = 0; // New property for progress bar

        // Navigation properties
        public Customer Customer { get; set; }

        // Assignment properties
        public int? DriverId { get; set; }
        public Driver? Driver { get; set; } // Navigation property for Driver

        public int? VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; } // Navigation property for Vehicle

        public ICollection<Assistant>? AssignedAssistants { get; set; } = new List<Assistant>(); // Collection for many-to-many relationship
    }
}