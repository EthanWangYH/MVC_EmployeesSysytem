using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeesSysytem.Data;
using EmployeesSysytem.Models;
using System.Security.Claims;
using EmployeesSysytem.Models.ViewModels;

namespace EmployeesSysytem.Controllers
{
    public class HolidaysController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HolidaysController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Holidays
        public async Task<IActionResult> Index()
        {
            HolidayViewModel holiday = new HolidayViewModel();
            holiday.Holidays = await _context.Holidays.ToListAsync();
            return View(holiday);
        }

        // GET: Holidays/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var holiday = await _context.Holidays
                .FirstOrDefaultAsync(m => m.Id == id);
            if (holiday == null)
            {
                return NotFound();
            }

            return View(holiday);
        }

        // GET: Holidays/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Holidays/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HolidayViewModel model)
        {
          try
            {
                var holiday = new Holiday();
                var userId = User.FindFirstValue(ClaimTypes.Name);
                holiday.CreatedOn = DateTime.Now;
                holiday.CreatedById = userId;
                holiday.Title = model.Title;
                holiday.Description = model.Description;
                holiday.StartTime = model.StartTime;
                holiday.EndTime = model.EndTime;
                _context.Add(holiday);
                await _context.SaveChangesAsync(userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View(model);
            }
        }

        // GET: Holidays/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            HolidayViewModel model = new HolidayViewModel();
            var holiday = await _context.Holidays.FindAsync(id);
            if (holiday == null)
            {
                return NotFound();
            }
            model.Title = holiday.Title;
            model.Description = holiday.Description;
            model.StartTime = holiday.StartTime;
            model.EndTime = holiday.EndTime;
            return View(model);
        }

        // POST: Holidays/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,HolidayViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.Name);
                    var existingHoliday = await _context.Holidays.FindAsync(id);
                    if (existingHoliday != null) {
                     existingHoliday.StartTime =  model.StartTime;
                        existingHoliday.EndTime = model.EndTime;
                        existingHoliday.Description = model.Description;
                        existingHoliday.Title = model.Title;
                        existingHoliday.ModifiedOn = DateTime.Now;
                        existingHoliday.ModifiedById = userId;
                        _context.Entry(existingHoliday).Property(x => x.CreatedById).IsModified = false;
                        _context.Entry(existingHoliday).Property(x=>x.CreatedOn).IsModified = false;
                        await _context.SaveChangesAsync(userId);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HolidayExists(model.Id))
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

        // GET: Holidays/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var holiday = await _context.Holidays
                .FirstOrDefaultAsync(m => m.Id == id);
            if (holiday == null)
            {
                return NotFound();
            }

            return View(holiday);
        }

        // POST: Holidays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.Name);
            var holiday = await _context.Holidays.FindAsync(id);
            if (holiday != null)
            {
                _context.Holidays.Remove(holiday);
            }

            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));
        }

        private bool HolidayExists(int id)
        {
            return _context.Holidays.Any(e => e.Id == id);
        }
    }
}
