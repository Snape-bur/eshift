using EShift.Data; // Ensure this matches your namespace for DbContext
using EShift.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System; // For DateTime.Now
using System.Linq;
using System.Threading.Tasks;

namespace EShift.Controllers // Use the same namespace as your existing AdminController if it's in a subfolder
{
    [Authorize(Roles = "Admin")] // Ensure only admins can access these actions
    public class AdminJobsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminJobsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminJobs (List all jobs for admin review with search)
        public async Task<IActionResult> Index(string searchTerm)
        {
            var jobsQuery = _context.Jobs
                                    .Include(j => j.Customer)
                                    .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                jobsQuery = jobsQuery.Where(j =>
                    j.CustomerName.Contains(searchTerm) ||
                    j.PickupAddress.Contains(searchTerm) ||
                    j.DeliveryAddress.Contains(searchTerm) ||
                    j.Status.Contains(searchTerm));
            }

            var jobs = await jobsQuery
                                .OrderBy(j => j.Status == "Pending" ? 0 :
                                              j.Status == "Approved" ? 1 : 2)
                                .ThenByDescending(j => j.CreatedAt)
                                .ToListAsync();

            return View(jobs);
        }


        // GET: Admin/AdminJobs/Details/5 (View a single job's details)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs
                                    .Include(j => j.Customer) // Include Customer data for details display
                                    .FirstOrDefaultAsync(m => m.JobId == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // POST: Admin/AdminJobs/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken] // Prevents Cross-Site Request Forgery attacks
        public async Task<IActionResult> Approve(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            // Only allow approval if the job is currently Pending
            if (job.Status == "Pending")
            {
                job.Status = "Approved";
                job.EstimatedArrival = job.MoveDate.Date; // You might want to set a more specific estimated arrival time here
                                                          // based on logic or a new admin input field later.
                                                          // For now, setting it to the move date as a placeholder if not set by customer.
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Job ID: {job.JobId} for {job.CustomerName} has been APPROVED.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Job ID: {job.JobId} cannot be approved as its current status is '{job.Status}'.";
            }

            return RedirectToAction(nameof(Details), new { id = job.JobId }); // Redirect back to job details
        }

        // POST: Admin/AdminJobs/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            // Only allow rejection if the job is currently Pending
            if (job.Status == "Pending")
            {
                job.Status = "Rejected";
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Job ID: {job.JobId} for {job.CustomerName} has been REJECTED.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Job ID: {job.JobId} cannot be rejected from its current status '{job.Status}'.";
            }

            return RedirectToAction(nameof(Details), new { id = job.JobId }); // Redirect back to job details
        }


        // GET: AdminJobs/Assign/5
        public async Task<IActionResult> Assign(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs
                                    .Include(j => j.Driver) // Include Driver for pre-selection
                                    .Include(j => j.Vehicle) // Include Vehicle for pre-selection
                                    .Include(j => j.AssignedAssistants) // Include assigned assistants
                                    .FirstOrDefaultAsync(m => m.JobId == id);

            if (job == null)
            {
                return NotFound();
            }

            // Fetch available resources based on their status
            var drivers = await _context.Drivers
                                        .Where(d => d.Status == DriverStatus.Available)
                                        .ToListAsync();
            var vehicles = await _context.Vehicles
                                         .Where(v => v.Status == VehicleStatus.Operational)
                                         .ToListAsync();
            var assistants = await _context.Assistants
                                          .Where(a => a.Status == AssistantStatus.Available)
                                          .ToListAsync();

            // Prepare the ViewModel
            var viewModel = new JobAssignmentModel
            {
                JobId = job.JobId,
                CustomerName = job.CustomerName,
                PickupAddress = job.PickupAddress,
                DeliveryAddress = job.DeliveryAddress,
                MoveDate = job.MoveDate,
                CurrentStatus = job.Status, // Set current status for display
                NewStatus = job.Status,     // Pre-select current status in the dropdown for editing
                DriverId = job.DriverId,
                VehicleId = job.VehicleId,
                SelectedAssistantIds = job.AssignedAssistants?.Select(a => a.AssistantId).ToList() ?? new List<int>(),
                EstimatedArrival = job.EstimatedArrival, // Set existing EstimatedArrival

                // Populate SelectLists for dropdowns
                Drivers = new SelectList(drivers, "DriverId", "FullName", job.DriverId),
                Vehicles = new SelectList(vehicles, "VehicleId", "LicensePlate", job.VehicleId),
                Assistants = new MultiSelectList(assistants, "AssistantId", "FullName", job.AssignedAssistants?.Select(a => a.AssistantId))
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(JobAssignmentModel model)
        {
            // DEBUG: Print all validation errors to console (optional)
            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                foreach (var error in state.Errors)
                {
                    Console.WriteLine($"{key}: {error.ErrorMessage}");
                }
            }

            // >>> THIS IS WHERE YOU NEED TO SET YOUR BREAKPOINT IN VISUAL STUDIO <<<
            // Hover your mouse over the gray margin to the left of this line,
            // and click to place a red circle (breakpoint).
            if (ModelState.IsValid)
            {
                try
                {
                    var jobToUpdate = await _context.Jobs
                                                    .Include(j => j.AssignedAssistants)
                                                    .FirstOrDefaultAsync(j => j.JobId == model.JobId);

                    if (jobToUpdate == null)
                    {
                        return NotFound();
                    }

                    // Update job properties from the view model
                    jobToUpdate.DriverId = model.DriverId;
                    jobToUpdate.VehicleId = model.VehicleId;
                    jobToUpdate.Status = model.NewStatus;
                    jobToUpdate.EstimatedArrival = model.EstimatedArrival ?? DateTime.Now;

                    // Handle AssignedAssistants
                    if (jobToUpdate.AssignedAssistants == null)
                        jobToUpdate.AssignedAssistants = new List<Assistant>();
                    else
                        jobToUpdate.AssignedAssistants.Clear();

                    if (model.SelectedAssistantIds != null && model.SelectedAssistantIds.Any())
                    {
                        var selectedAssistants = await _context.Assistants
                            .Where(a => model.SelectedAssistantIds.Contains(a.AssistantId))
                            .ToListAsync();

                        foreach (var assistant in selectedAssistants)
                        {
                            jobToUpdate.AssignedAssistants.Add(assistant);
                        }
                    }

                    jobToUpdate.ProgressPercentage = GetProgressPercentage(jobToUpdate.Status);

                    _context.Update(jobToUpdate);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Job assigned successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobExists(model.JobId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error assigning job: {ex.Message}");
                    ModelState.AddModelError("", "An unexpected error occurred while assigning the job. Please try again.");
                }
            }

            // If ModelState is not valid or an error occurred, repopulate dropdowns and return the view
            await PopulateAssignmentViewModelDropdowns(model);
            return View(model);
        }

        private bool JobExists(int id)
        {
            return _context.Jobs.Any(e => e.JobId == id);
        }

        // Helper method to repopulate dropdowns for the view model
        private async Task PopulateAssignmentViewModelDropdowns(JobAssignmentModel viewModel)
        {
            var drivers = await _context.Drivers.Where(d => d.Status == DriverStatus.Available).ToListAsync();
            var vehicles = await _context.Vehicles.Where(v => v.Status == VehicleStatus.Operational).ToListAsync();
            var assistants = await _context.Assistants.Where(a => a.Status == AssistantStatus.Available).ToListAsync();

            viewModel.Drivers = new SelectList(drivers, "DriverId", "FullName", viewModel.DriverId);
            viewModel.Vehicles = new SelectList(vehicles, "VehicleId", "LicensePlate", viewModel.VehicleId);
            viewModel.Assistants = new MultiSelectList(assistants, "AssistantId", "FullName", viewModel.SelectedAssistantIds);

            // Also ensure current job details are re-populated for display on error
            var job = await _context.Jobs.FirstOrDefaultAsync(j => j.JobId == viewModel.JobId);
            if (job != null)
            {
                viewModel.CustomerName = job.CustomerName;
                viewModel.PickupAddress = job.PickupAddress;
                viewModel.DeliveryAddress = job.DeliveryAddress;
                viewModel.MoveDate = job.MoveDate;
                viewModel.CurrentStatus = job.Status;
                // NewStatus is already from model, no need to reset from job unless you want to revert on error
            }
        }

        // Helper method for progress percentage
        private int GetProgressPercentage(string status)
        {
            return status switch
            {
                "Pending" => 0,
                "Approved" => 25,
                "In Progress" => 50,
                "Completed" => 100,
                "Rejected" => 0,
                "Canceled" => 0,
                "Assigned" => 25, // Assuming "Assigned" is a new status after "Approved"
                _ => 0,
            };
        }
    }
}