using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EShift.Data;
using EShift.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace EShift.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CustomersController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Customers/Index
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var customers = await _context.Customers
                .Include(c => c.Jobs) // Include jobs
                .Where(c => c.AppUserId == userId)
                .ToListAsync();
            return View(customers);
        }

        // POST: Customers/CreateJob
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateJob(JobRequestModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var job = new Job
                {
                    AppUserId = userId,
                    CustomerName = model.CustomerName,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    PickupAddress = model.PickupAddress,
                    DeliveryAddress = model.DeliveryAddress,
                    MoveDate = model.MoveDate,
                    MoveTime = model.MoveTime,
                    Category = model.Category,
                    ItemCount = model.ItemCount,
                    EstimatedKg = model.EstimatedKg,
                    Instructions = model.Instructions,
                    Status = "Pending", // For admin approval
                    CreatedAt = DateTime.UtcNow,
                    EstimatedArrival = model.MoveDate.AddHours(2), // Placeholder
                    ProgressPercentage = 0
                };

                _context.Jobs.Add(job);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Job request submitted successfully!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Please correct the errors in the form.";
            return RedirectToAction(nameof(Index));
        }



        // GET: /Customer/Profile
        [HttpGet]
        public IActionResult Profile()
        {
            var userId = _userManager.GetUserId(User);
            var customer = _context.Customers.FirstOrDefault(c => c.AppUserId == userId);
            return View(customer);
        }

        // POST: /Customer/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(Customer updatedCustomer, string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            var customer = _context.Customers.FirstOrDefault(c => c.AppUserId == userId);

            if (customer == null || user == null)
                return NotFound();

            // Update fields
            customer.FullName = updatedCustomer.FullName;
            customer.Address = updatedCustomer.Address;
            customer.PhoneNumber = updatedCustomer.PhoneNumber;

            if (!string.IsNullOrEmpty(CurrentPassword) && !string.IsNullOrEmpty(NewPassword))
            {
                if (NewPassword != ConfirmPassword)
                {
                    TempData["Error"] = "New password and confirm password do not match.";
                    return RedirectToAction("Profile");
                }

                var passwordValid = await _userManager.CheckPasswordAsync(user, CurrentPassword);
                if (!passwordValid)
                {
                    TempData["Error"] = "Current password is incorrect.";
                    return RedirectToAction("Profile");
                }

                var result = await _userManager.ChangePasswordAsync(user, CurrentPassword, NewPassword);
                if (!result.Succeeded)
                {
                    // Collect all password validation error messages
                    var errorMessages = string.Join(" ", result.Errors.Select(e => e.Description));
                    TempData["Error"] = $"Failed to change password: {errorMessages}";
                    return RedirectToAction("Profile");
                }
            }


            await _context.SaveChangesAsync();
            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }




        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.AppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        public IActionResult Create()
        {
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,PhoneNumber,Address,AppUserId")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id", customer.AppUserId);
            return View(customer);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id", customer.AppUserId);
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,PhoneNumber,Address,AppUserId")] Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id", customer.AppUserId);
            return View(customer);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.AppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Customers' is null.");
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return (_context.Customers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}