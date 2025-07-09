using System.ComponentModel.DataAnnotations; 
using System.Collections.Generic; 

namespace EShift.Models
{
    
    public enum AssistantStatus
    {
        Available,    // Assistant is ready and not assigned to a job
        OnJob,        // Assistant is currently assigned to and working on a job
        OffDuty,      // Assistant is not working (e.g., day off, end of shift)
        OnBreak,
        SickLeave,    // Assistant is unavailable due to sickness
    }

    public class Assistant
    {
        [Key] // Marks AssistantId as the primary key
        [Required] // Ensures the ID is always present
        public int AssistantId { get; set; }

        [Required] // Ensures FullName is always present
        public string FullName { get; set; }

        [Required] // Ensures a status is always set for the assistant
        public AssistantStatus Status { get; set; } // Using enum for clear, predefined assistant states

        public string? PhoneNumber { get; set; } // Optional: Made nullable as contact might not always be required for the model

        public ICollection<Job>? AssignedJobs { get; set; }
    }
}