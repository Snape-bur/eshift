using System.IO;
using ClosedXML.Excel;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Layout;
using iText.Layout.Element;

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EShift.Data;
using EShift.Models.ViewModels;
using EShift.Models;
using Microsoft.AspNetCore.Mvc; // Ensure this is the correct namespace for your Job model

namespace EShift.Controllers
{
   
    [Authorize(Roles = "Admin")] // Restrict access to admins
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // --- 1. Calculate Date Ranges for Reports ---
            var now = DateTime.Now;

            // Start of the current week (Sunday)
            var startOfThisWeek = now.Date.AddDays(-(int)now.DayOfWeek);

            // Start of the current month
            var startOfThisMonth = new DateTime(now.Year, now.Month, 1);

            // Start of the previous month
            var startOfLastMonth = startOfThisMonth.AddMonths(-1);
            var endOfLastMonth = startOfThisMonth.AddDays(-1);

            // --- 2. Fetch Job Counts by Status (using your string values) ---
            var totalJobs = await _context.Jobs.CountAsync();
            var pendingJobs = await _context.Jobs.CountAsync(j => j.Status == "Pending");
            var approvedJobs = await _context.Jobs.CountAsync(j => j.Status == "Approved");
            var assignedJobs = await _context.Jobs.CountAsync(j => j.Status == "Assigned");
            var inProgressJobs = await _context.Jobs.CountAsync(j => j.Status == "In Progress");
            var completedJobs = await _context.Jobs.CountAsync(j => j.Status == "Completed");
            var rejectedJobs = await _context.Jobs.CountAsync(j => j.Status == "Rejected");
            var canceledJobs = await _context.Jobs.CountAsync(j => j.Status == "Cancelled");

            // --- 3. Fetch Time-Based Counts ---
            var jobsCreatedThisWeek = await _context.Jobs
                .CountAsync(j => j.CreatedAt.Date >= startOfThisWeek);

            var jobsCreatedThisMonth = await _context.Jobs
                .CountAsync(j => j.CreatedAt.Date >= startOfThisMonth);

            var completedJobsThisMonth = await _context.Jobs
                .CountAsync(j => j.Status == "Completed" && j.CreatedAt.Date >= startOfThisMonth);

            var completedJobsLastMonth = await _context.Jobs
                .CountAsync(j => j.Status == "Completed" && j.CreatedAt.Date >= startOfLastMonth && j.CreatedAt.Date <= endOfLastMonth);

            // --- 4. Fetch Recent Job History (e.g., last 10 jobs) ---
            // Assuming your Job model has a CreatedAt or LastUpdated field for sorting
            var recentJobs = await _context.Jobs
                .OrderByDescending(j => j.CreatedAt) // Or by MoveDate, if that's what you prefer
                .Take(10)
                .ToListAsync();

            // --- 5. Populate the ViewModel and Pass it to the View ---
            var viewModel = new AdminDashboardViewModel
            {
                // Overall Stats
                TotalJobs = totalJobs,
                PendingJobs = pendingJobs,
                ApprovedJobs = approvedJobs,
                AssignedJobs = assignedJobs,
                InProgressJobs = inProgressJobs,
                CompletedJobs = completedJobs,
                RejectedJobs = rejectedJobs,
                CanceledJobs = canceledJobs,

                // Time-based Reports
                JobsCreatedThisWeek = jobsCreatedThisWeek,
                JobsCreatedThisMonth = jobsCreatedThisMonth,
                CompletedJobsThisMonth = completedJobsThisMonth,
                CompletedJobsLastMonth = completedJobsLastMonth,

                // Job History
                RecentJobs = recentJobs
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> ExportToExcel()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Dashboard Report");

            worksheet.Cell(1, 1).Value = "Metric";
            worksheet.Cell(1, 2).Value = "Value";

            // Move all DateTime calculations outside the LINQ
            var now = DateTime.Now;
            var startOfWeek = now.Date.AddDays(-(int)now.DayOfWeek);
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var lastMonth = now.AddMonths(-1);
            var startOfLastMonth = new DateTime(lastMonth.Year, lastMonth.Month, 1);
            var endOfLastMonth = startOfMonth.AddDays(-1);

            var stats = new List<(string Label, int Value)>
    {
        ("Total Jobs", await _context.Jobs.CountAsync()),
        ("Pending Jobs", await _context.Jobs.CountAsync(j => j.Status == "Pending")),
        ("Approved Jobs", await _context.Jobs.CountAsync(j => j.Status == "Approved")),
        ("Assigned Jobs", await _context.Jobs.CountAsync(j => j.Status == "Assigned")),
        ("In Progress Jobs", await _context.Jobs.CountAsync(j => j.Status == "In Progress")),
        ("Completed Jobs", await _context.Jobs.CountAsync(j => j.Status == "Completed")),
        ("Rejected Jobs", await _context.Jobs.CountAsync(j => j.Status == "Rejected")),
        ("Canceled Jobs", await _context.Jobs.CountAsync(j => j.Status == "Cancelled")),

        // Time-based stats using precomputed dates
        ("Jobs Created This Week", await _context.Jobs.CountAsync(j => j.CreatedAt >= startOfWeek)),
        ("Jobs Created This Month", await _context.Jobs.CountAsync(j => j.CreatedAt >= startOfMonth)),
        ("Completed Jobs This Month", await _context.Jobs.CountAsync(j => j.Status == "Completed" && j.CreatedAt >= startOfMonth)),
        ("Completed Jobs Last Month", await _context.Jobs.CountAsync(j =>
            j.Status == "Completed" &&
            j.CreatedAt >= startOfLastMonth &&
            j.CreatedAt <= endOfLastMonth))
    };

            // Write data to Excel
            for (int i = 0; i < stats.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = stats[i].Label;
                worksheet.Cell(i + 2, 2).Value = stats[i].Value;
            }

            // Export as file
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "AdminDashboardReport.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> ExportToPdf()
        {
            var jobs = await _context.Jobs
                .OrderByDescending(j => j.CreatedAt)
                .Take(10)
                .ToListAsync();

            using var ms = new MemoryStream();
            using (var writer = new iText.Kernel.Pdf.PdfWriter(ms))
            {
                var pdf = new iText.Kernel.Pdf.PdfDocument(writer);
                var doc = new iText.Layout.Document(pdf);

                // Create bold font for title
                var boldFont = iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);

                // Add styled title paragraph
                var title = new iText.Layout.Element.Paragraph("Recent Job Report")
                    .SetFont(boldFont)
                    .SetFontSize(16);
                doc.Add(title);

                // Create table with headers
                var table = new iText.Layout.Element.Table(4).UseAllAvailableWidth();
                table.AddHeaderCell("Job ID");
                table.AddHeaderCell("Customer");
                table.AddHeaderCell("Move Date");
                table.AddHeaderCell("Status");

                // Fill table rows
                foreach (var job in jobs)
                {
                    table.AddCell($"#{job.JobId}");
                    table.AddCell(job.CustomerName ?? "-");
                    table.AddCell(job.MoveDate.ToString("MMM dd, yyyy"));
                    table.AddCell(job.Status);
                }

                doc.Add(table);
                doc.Close(); // Finalize PDF
            }

            return File(ms.ToArray(), "application/pdf", "RecentJobsReport.pdf");
        }


    }
}