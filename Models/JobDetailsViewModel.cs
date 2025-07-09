// EShift.ViewModels/JobDetailsViewModel.cs
using EShift.Models;
using System;
using System.Collections.Generic;
using System.Linq; // For .Any() in AssistantsCountText

namespace EShift.ViewModels
{
    public class JobDetailsViewModel
    {
        public Job Job { get; set; } // The primary Job object, which already includes Customer and others if included in query

        // Derived properties for UI display logic (optional but good practice)
        public string StatusCssClass
        {
            get
            {
                return Job?.Status switch // Use null-conditional operator for safety
                {
                    "Pending" => "text-yellow-600",
                    "Approved" => "text-green-600",
                    "In Progress" => "text-blue-600",
                    "Completed" => "text-purple-600", // Changed to purple for clarity
                    "Rejected" => "text-red-500",
                    "Canceled" => "text-gray-500", // Use Canceled for consistency
                    _ => "text-gray-600", // Default
                };
            }
        }

        public string FormattedMoveDate => Job?.MoveDate.ToString("MMMM dd, yyyy");
        public string FormattedEstimatedArrival => Job?.EstimatedArrival.ToString("yyyy-MM-dd HH:mm") ?? "Not yet estimated";

        public bool CanCancel => Job?.Status == "Pending" || Job?.Status == "Approved"; // Only allow cancellation if Pending or Approved
        public bool CanEdit => Job?.Status == "Pending"; // Only allow editing if Pending

        public string AssistantsCountText
        {
            get
            {
                if (Job?.AssignedAssistants != null && Job.AssignedAssistants.Any())
                {
                    return $"{Job.AssignedAssistants.Count} Assistant(s) assigned.";
                }
                return "No assistants assigned.";
            }
        }
    }
}