using EShift.Data;
using EShift.Models;
using EShift.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EShift.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminCustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminCustomerController> _logger;

        public AdminCustomerController(ApplicationDbContext context, ILogger<AdminCustomerController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: AdminCustomer
        public async Task<IActionResult> Index(string searchTerm)
        {
            var query = _context.Customers
                .Include(c => c.AppUser)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();

                query = query.Where(c =>
                    c.FullName.ToLower().Contains(searchTerm) ||
                    c.PhoneNumber.ToLower().Contains(searchTerm) ||
                    c.Address.ToLower().Contains(searchTerm) ||
                    c.AppUser.UserName.ToLower().Contains(searchTerm) ||
                    c.AppUser.Email.ToLower().Contains(searchTerm)
                );
            }

            ViewData["CurrentFilter"] = searchTerm;

            var customers = await query.ToListAsync();
            return View(customers);
        }


        // GET: AdminCustomer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers
                .Include(c => c.AppUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (customer == null) return NotFound();

            return View(customer);
        }

        // GET: AdminCustomer/Create
        public IActionResult Create()
        {
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: AdminCustomer/Create
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

            _logger.LogWarning("Customer creation failed due to validation errors.");
            foreach (var modelStateEntry in ModelState.Values)
            {
                foreach (var error in modelStateEntry.Errors)
                {
                    _logger.LogError($"Validation Error: {error.ErrorMessage}");
                }
            }

            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Email", customer.AppUserId);
            return View(customer);
        }

        // GET: AdminCustomer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            // Create and populate the ViewModel
            var viewModel = new EditCustomerViewModel
            {
                Id = customer.Id,
                FullName = customer.FullName,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address
                // Removed: AppUserId = customer.AppUserId; // No longer in ViewModel
                // Removed: AppUsersList = new SelectList(_context.Users, "Id", "Email", customer.AppUserId); // No longer in ViewModel
            };

            return View(viewModel); // Pass the ViewModel to the view
        }

        // POST: AdminCustomer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCustomerViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var customerToUpdate = await _context.Customers.FindAsync(viewModel.Id);

                    if (customerToUpdate == null)
                    {
                        return NotFound();
                    }

                    customerToUpdate.FullName = viewModel.FullName;
                    customerToUpdate.PhoneNumber = viewModel.PhoneNumber;
                    customerToUpdate.Address = viewModel.Address;

                    // IMPORTANT: As discussed, AppUserId is NOT updated from the form.
                    // It retains its original value from 'customerToUpdate' which was fetched from DB.
                    // DO NOT put 'customerToUpdate.AppUserId = viewModel.AppUserId;' here.

                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Customer with ID {viewModel.Id} updated successfully.");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, $"Concurrency error updating customer with ID {viewModel.Id}.");
                    if (!CustomerExists(viewModel.Id))
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
                    _logger.LogError(ex, $"An unexpected error occurred while updating customer with ID {viewModel.Id}.");
                    ModelState.AddModelError("", "An unexpected error occurred while saving. Please try again.");
                }
                return RedirectToAction(nameof(Index));
            }

            // --- CRITICAL FIX FOR CS0117 / CS1061 ERRORS ---
            // If validation fails, we no longer have AppUsersList or AppUserId on the ViewModel to re-populate.
            // Just return the view with the current ViewModel and its errors.
            // REMOVE the line that was trying to set viewModel.AppUsersList:
            // viewModel.AppUsersList = new SelectList(_context.Users, "Id", "Email", viewModel.AppUserId); // DELETE THIS LINE

            _logger.LogWarning($"Customer with ID {viewModel.Id} update failed due to validation errors.");
            foreach (var modelStateEntry in ModelState.Values)
            {
                foreach (var error in modelStateEntry.Errors)
                {
                    _logger.LogError($"Validation Error: {error.ErrorMessage}");
                }
            }

            return View(viewModel); // Pass the ViewModel back to the view
        }


        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }


        // GET: AdminCustomer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers
                .Include(c => c.AppUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (customer == null) return NotFound();

            return View(customer);
        }

        // POST: AdminCustomer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Customer with ID {id} deleted successfully.");
            }
            else
            {
                _logger.LogWarning($"Attempted to delete non-existent customer with ID {id}.");
            }

            return RedirectToAction(nameof(Index));
        }

    }
}