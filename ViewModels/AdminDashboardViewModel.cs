using System.Collections.Generic;
using EShift.Models; // Make sure this path is correct for your Job model

namespace EShift.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        // Overall Job Status Distribution
        public int TotalJobs { get; set; }
        public int PendingJobs { get; set; }
        public int ApprovedJobs { get; set; }
        public int AssignedJobs { get; set; }
        public int InProgressJobs { get; set; }
        public int CompletedJobs { get; set; }
        public int RejectedJobs { get; set; }
        public int CanceledJobs { get; set; }

        // Time-based Statistics for reporting
        public int JobsCreatedThisWeek { get; set; }
        public int JobsCreatedThisMonth { get; set; }
        public int CompletedJobsThisMonth { get; set; }
        public int CompletedJobsLastMonth { get; set; }

        // A list for the "Job History" table
        public List<Job> RecentJobs { get; set; }
    }
}