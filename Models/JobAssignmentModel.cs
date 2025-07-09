// EShift.Models/JobAssignmentModel.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EShift.Models
{
    public class JobAssignmentModel
    {
        public int JobId { get; set; }

        // PROPERTIES TO DISPLAY CURRENT JOB INFORMATION (READ-ONLY IN THE FORM)
        // >>> MAKE THESE NULLABLE (add '?') SO THEY ARE NOT IMPLICITLY REQUIRED <<<
        [Display(Name = "Customer Name")]
        public string? CustomerName { get; set; } // Changed to string?

        [Display(Name = "Pickup Address")]
        public string? PickupAddress { get; set; } // Changed to string?

        [Display(Name = "Delivery Address")]
        public string? DeliveryAddress { get; set; } // Changed to string?

        [Display(Name = "Move Date")]
        [DataType(DataType.Date)]
        public DateTime? MoveDate { get; set; } // Changed to DateTime?

        [Display(Name = "Current Status")]
        public string? CurrentStatus { get; set; } // Changed to string?

        // PROPERTIES FOR ACTUAL ASSIGNMENT (EDITABLE BY ADMIN)
        // These remain as you had them, with [Required] if they are actual user inputs
        [Required(ErrorMessage = "Please select a new status.")]
        [Display(Name = "New Status")]
        public string NewStatus { get; set; }

        [Display(Name = "Driver")]
        // If selecting a driver is mandatory, add [Required] here. Otherwise, leave it nullable without [Required].
        public int? DriverId { get; set; }

        [Display(Name = "Vehicle")]
        // If selecting a vehicle is mandatory, add [Required] here. Otherwise, leave it nullable without [Required].
        public int? VehicleId { get; set; }

        [Display(Name = "Assistants")]
        public List<int>? SelectedAssistantIds { get; set; } = new List<int>(); // For multi-select

        [Display(Name = "Estimated Arrival")]
        [DataType(DataType.DateTime)] // Ensures correct HTML input type
        // If providing an estimated arrival is mandatory, add [Required] here. Otherwise, leave it nullable without [Required].
        public DateTime? EstimatedArrival { get; set; }

        // For dropdowns and multiselect list items (populated by the controller)
        public SelectList? Drivers { get; set; }
        public SelectList? Vehicles { get; set; }
        public MultiSelectList? Assistants { get; set; }
    }
}