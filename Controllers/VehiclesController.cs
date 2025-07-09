using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EShift.Data;
using EShift.Models;

namespace EShift.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VehiclesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vehicles
        public async Task<IActionResult> Index(string searchTerm)
        {
            if (_context.Vehicles == null)
                return Problem("Entity set 'ApplicationDbContext.Vehicles' is null.");

            // Get all vehicles first
            var allVehicles = await _context.Vehicles.ToListAsync();

            // Filter in-memory if search term exists
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower(); // make search case-insensitive

                allVehicles = allVehicles
                    .Where(v =>
                        (!string.IsNullOrEmpty(v.Make) && v.Make.ToLower().Contains(searchTerm)) ||
                        (!string.IsNullOrEmpty(v.Model) && v.Model.ToLower().Contains(searchTerm)) ||
                        (!string.IsNullOrEmpty(v.LicensePlate) && v.LicensePlate.ToLower().Contains(searchTerm)) ||
                        v.Type.ToString().ToLower().Contains(searchTerm)
                    )
                    .ToList();
            }

            ViewData["CurrentFilter"] = searchTerm;
            return View(allVehicles);
        }




        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Vehicles == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(m => m.VehicleId == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // GET: Vehicles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VehicleId,Type,Make,Model,Color,CapacityCubicFeet,Status,LicensePlate")] Vehicle vehicle)
        {
            var maxCapacities = new Dictionary<string, int>
    {
        { "MotorCycle", 20 }, // Match enum name MotorCycle
        { "FourDoorSedan", 100 }, // Match enum name FourDoorSedan
        { "FiveDoorCar", 200 }, // Match enum name FiveDoorCar
        { "PickupTruck", 1100 }, // Match enum name PickupTruck
        { "MultiPurposeVehicle", 300 }, // Match enum name MultiPurposeVehicle
        { "SixWheelFusoTruck", 2400 } // Match enum name SixWheelFusoTruck
    };

            // Convert VehicleType enum to string for dictionary lookup
            string vehicleTypeString = vehicle.Type.ToString();
            if (maxCapacities.ContainsKey(vehicleTypeString))
            {
                if (vehicle.CapacityCubicFeet < 1 || vehicle.CapacityCubicFeet > maxCapacities[vehicleTypeString])
                {
                    ModelState.AddModelError("CapacityCubicFeet", $"Capacity for {vehicleTypeString} must be between 1 and {maxCapacities[vehicleTypeString]} cubic feet.");
                }
            }
            else
            {
                ModelState.AddModelError("Type", "Invalid vehicle type.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vehicle);
        }

        [HttpGet] // This explicitly marks it for GET requests
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound(); // No ID provided, so we don't know which vehicle to edit
            }

            // Find the vehicle by ID
            var vehicle = await _context.Vehicles.FindAsync(id);

            if (vehicle == null)
            {
                return NotFound(); // Vehicle with this ID does not exist
            }

            // Pass the found vehicle object to the Edit view to populate the form
            return View(vehicle);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VehicleId,Type,Make,Model,Color,CapacityCubicFeet,Status,LicensePlate")] Vehicle vehicle)
        {
            if (id != vehicle.VehicleId)
            {
                return NotFound();
            }

            var maxCapacities = new Dictionary<string, int>
    {
        { "MotorCycle", 20 },
        { "FourDoorSedan", 100 },
        { "FiveDoorCar", 200 },
        { "PickupTruck", 1100 },
        { "MultiPurposeVehicle", 300 },
        { "SixWheelFusoTruck", 2400 }
    };

            // Convert VehicleType enum to string for dictionary lookup
            string vehicleTypeString = vehicle.Type.ToString();
            if (maxCapacities.ContainsKey(vehicleTypeString))
            {
                if (vehicle.CapacityCubicFeet < 1 || vehicle.CapacityCubicFeet > maxCapacities[vehicleTypeString])
                {
                    ModelState.AddModelError("CapacityCubicFeet", $"Capacity for {vehicleTypeString} must be between 1 and {maxCapacities[vehicleTypeString]} cubic feet.");
                }
            }
            else
            {
                ModelState.AddModelError("Type", "Invalid vehicle type.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.VehicleId))
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
            return View(vehicle);
        }

        // GET: Vehicles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Vehicles == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(m => m.VehicleId == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Vehicles == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Vehicles'  is null.");
            }
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
          return (_context.Vehicles?.Any(e => e.VehicleId == id)).GetValueOrDefault();
        }
    }
}
