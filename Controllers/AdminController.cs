
using EShift.Data;
using EShift.Models;
using EShift.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public AdminController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    // GET: Admin
    public IActionResult Index()
    {
        var admins = _context.Admins.Include(a => a.AppUser).ToList();
        return View(admins);
    }

    // GET: Admin/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/Create
    // POST: Admin/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AddAdminViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                EmailConfirmed = true // <<=== ADD THIS LINE!
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");

                var admin = new Admin
                {
                    AppUserId = user.Id,
                    FullName = model.FullName,
                    Department = model.Department
                };

                _context.Admins.Add(admin);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        return View(model);
    }

    // GET: Admin/Details/{id}
    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var admin = await _context.Admins
            .Include(a => a.AppUser)
            .FirstOrDefaultAsync(a => a.AppUserId == id);

        if (admin == null)
        {
            return NotFound();
        }

        return View(admin);
    }

    // GET: Admin/Edit/{id}
    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var admin = await _context.Admins
            .Include(a => a.AppUser)
            .FirstOrDefaultAsync(a => a.AppUserId == id);

        if (admin == null)
        {
            return NotFound();
        }

        var model = new EditAdminViewModel
        {
            AppUserId = admin.AppUserId,
            FullName = admin.FullName,
            Department = admin.Department,
            Email = admin.AppUser?.Email
        };

        return View(model);
    }

    // POST: Admin/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, EditAdminViewModel model)
    {
        if (id != model.AppUserId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var admin = await _context.Admins
                .Include(a => a.AppUser)
                .FirstOrDefaultAsync(a => a.AppUserId == id);

            if (admin == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Update admin and user details
            admin.FullName = model.FullName;
            admin.Department = model.Department;
            user.FullName = model.FullName;
            user.Email = model.Email;
            user.UserName = model.Email;

            _context.Admins.Update(admin);
            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    // GET: Admin/Delete/{id}
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var admin = await _context.Admins
            .Include(a => a.AppUser)
            .FirstOrDefaultAsync(a => a.AppUserId == id);

        if (admin == null)
        {
            return NotFound();
        }

        return View(admin);
    }

    // POST: Admin/Delete/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var admin = await _context.Admins
            .FirstOrDefaultAsync(a => a.AppUserId == id);

        if (admin == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);

        // Remove admin record first (EF-managed)
        _context.Admins.Remove(admin);

        try
        {
            await _context.SaveChangesAsync(); // Persist removal of Admin entry
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Admins.Any(a => a.AppUserId == id))
            {
                return NotFound(); // Already deleted by another process
            }

            throw; // Unhandled concurrency issue
        }

        // Then try deleting Identity user (UserManager-managed)
        if (user != null)
        {
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                // You could log errors or show a user-friendly message
                ModelState.AddModelError("", "Failed to delete the associated user.");
                return View(admin);
            }
        }

        return RedirectToAction(nameof(Index));
    }

}