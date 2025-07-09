using EShift.Data;
using EShift.Models;
using EShift.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EShift.Controllers
{
    [Authorize(Roles = "Customer")]
    public class JobsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public JobsController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Action to display job details
        [HttpGet]
        public async Task<IActionResult> Details(int? jobId)
        {
            if (jobId == null)
            {
                // Optionally log this or redirect to a more general error page
                TempData["ErrorMessage"] = "Job ID is missing.";
                return RedirectToAction("Index", "Customers"); // Redirect to your customer dashboard
            }

            // Fetch the job including all its navigation properties
            var job = await _context.Jobs
                                    .Include(j => j.Customer)          // Include the associated Customer
                                    .Include(j => j.Driver)            // Include the assigned Driver
                                    .Include(j => j.Vehicle)           // Include the assigned Vehicle
                                    .Include(j => j.AssignedAssistants) // Include the assigned Assistants
                                    .FirstOrDefaultAsync(m => m.JobId == jobId);

            if (job == null)
            {
                TempData["ErrorMessage"] = "Job not found.";
                return RedirectToAction("Index", "Customers"); // Redirect to your customer dashboard
            }

            // Create and populate the ViewModel
            var viewModel = new JobDetailsViewModel
            {
                Job = job // Assign the fully loaded job object
            };

            return View(viewModel);
        }

        // POST: Jobs/Cancel/{jobId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int jobId)
        {
            var userId = _userManager.GetUserId(User);
            var job = await _context.Jobs
                .FirstOrDefaultAsync(j => j.JobId == jobId && j.AppUserId == userId && j.Status == "Pending");

            if (job == null)
            {
                TempData["ErrorMessage"] = "Job not found or cannot be cancelled.";
                return RedirectToAction("Index", "Customers");
            }

            job.Status = "Cancelled";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Job cancelled successfully.";
            return RedirectToAction("Index", "Customers");
        }


      

        // Helper method to check if a job exists (can be reused)
        private bool JobExists(int id)
        {
            return _context.Jobs.Any(e => e.JobId == id);
        }
    }
}