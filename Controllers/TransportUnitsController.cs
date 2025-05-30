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
    public class TransportUnitsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransportUnitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TransportUnits
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TransportUnits.Include(t => t.Driver);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TransportUnits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TransportUnits == null)
            {
                return NotFound();
            }

            var transportUnit = await _context.TransportUnits
                .Include(t => t.Driver)
                .FirstOrDefaultAsync(m => m.VehicleId == id);
            if (transportUnit == null)
            {
                return NotFound();
            }

            return View(transportUnit);
        }

        // GET: TransportUnits/Create
        public IActionResult Create()
        {
            ViewData["DriverId"] = new SelectList(_context.Drivers, "Id", "FullName");
            return View();
        }

        // POST: TransportUnits/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VehicleId,LicensePlate,VehicleType,Make,Model,Year,Capacity,Status,DriverId")] TransportUnit transportUnit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transportUnit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DriverId"] = new SelectList(_context.Drivers, "Id", "FullName", transportUnit.DriverId);
            return View(transportUnit);
        }

        // GET: TransportUnits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TransportUnits == null)
            {
                return NotFound();
            }

            var transportUnit = await _context.TransportUnits.FindAsync(id);
            if (transportUnit == null)
            {
                return NotFound();
            }
            ViewData["DriverId"] = new SelectList(_context.Drivers, "Id", "FullName", transportUnit.DriverId);
            return View(transportUnit);
        }

        // POST: TransportUnits/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VehicleId,LicensePlate,VehicleType,Make,Model,Year,Capacity,Status,DriverId")] TransportUnit transportUnit)
        {
            if (id != transportUnit.VehicleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transportUnit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransportUnitExists(transportUnit.VehicleId))
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
            ViewData["DriverId"] = new SelectList(_context.Drivers, "Id", "FullName", transportUnit.DriverId);
            return View(transportUnit);
        }

        // GET: TransportUnits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TransportUnits == null)
            {
                return NotFound();
            }

            var transportUnit = await _context.TransportUnits
                .Include(t => t.Driver)
                .FirstOrDefaultAsync(m => m.VehicleId == id);
            if (transportUnit == null)
            {
                return NotFound();
            }

            return View(transportUnit);
        }

        // POST: TransportUnits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TransportUnits == null)
            {
                return Problem("Entity set 'ApplicationDbContext.TransportUnits'  is null.");
            }
            var transportUnit = await _context.TransportUnits.FindAsync(id);
            if (transportUnit != null)
            {
                _context.TransportUnits.Remove(transportUnit);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransportUnitExists(int id)
        {
          return (_context.TransportUnits?.Any(e => e.VehicleId == id)).GetValueOrDefault();
        }
    }
}
