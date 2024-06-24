using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeesSysytem.Data;
using EmployeesSysytem.Models;
using System.Security.Claims;

namespace EmployeesSysytem.Controllers
{
    public class SystemProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SystemProfilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SystemProfiles
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SystemProfiles.Include(s => s.Profile);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: SystemProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemProfile = await _context.SystemProfiles
                .Include(s => s.Profile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (systemProfile == null)
            {
                return NotFound();
            }

            return View(systemProfile);
        }

        // GET: SystemProfiles/Create
        public IActionResult Create()
        {
            ViewData["ProfileId"] = new SelectList(_context.SystemProfiles, "Id", "Name");
            return View();
        }

        // POST: SystemProfiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SystemProfile systemProfile)
        {
            ViewData["ProfileId"] = new SelectList(_context.SystemProfiles, "Id", "Name");
            var userId = User.FindFirstValue(ClaimTypes.Name);
            var user = _context.ApplicationUsers.FirstOrDefault(u => u.UserName == userId);
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                systemProfile.CreatedOn= DateTime.Now;
                systemProfile.CreatedById = user!.FullName??null;
                _context.Add(systemProfile);
                await _context.SaveChangesAsync(userId);
                return RedirectToAction(nameof(Index));
            }          
            return View(systemProfile);
        }

        // GET: SystemProfiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemProfile = await _context.SystemProfiles.FindAsync(id);
            if (systemProfile == null)
            {
                return NotFound();
            }
            ViewData["ProfileId"] = new SelectList(_context.SystemProfiles, "Id", "Name");
            return View(systemProfile);
        }

        // POST: SystemProfiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ProfileId,Order,CreatedById,CreatedOn,ModifiedById,ModifiedOn")] SystemProfile systemProfile)
        {
            if (id != systemProfile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.Name);
                    var existingSystemProfile = await _context.SystemProfiles.FindAsync(id);
                    if (existingSystemProfile != null) {
                     existingSystemProfile.Name = systemProfile.Name;
                        existingSystemProfile.ProfileId = systemProfile.ProfileId;
                        existingSystemProfile.Order = systemProfile.Order;
                        existingSystemProfile.ModifiedById = userId;
                        existingSystemProfile.ModifiedOn = DateTime.Now;
                        _context.Entry(existingSystemProfile).Property(x => x.CreatedById).IsModified = false;
                        _context.Entry(existingSystemProfile).Property(x=>x.CreatedOn).IsModified = false;
                        await _context.SaveChangesAsync(userId);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SystemProfileExists(systemProfile.Id))
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
            ViewData["ProfileId"] = new SelectList(_context.SystemProfiles, "Id", "Name");
            return View(systemProfile);
        }

        // GET: SystemProfiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemProfile = await _context.SystemProfiles
                .Include(s => s.Profile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (systemProfile == null)
            {
                return NotFound();
            }

            return View(systemProfile);
        }

        // POST: SystemProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.Name);
            var systemProfile = await _context.SystemProfiles.FindAsync(id);
            if (systemProfile != null)
            {
                _context.SystemProfiles.Remove(systemProfile);
            }

            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));
        }

        private bool SystemProfileExists(int id)
        {
            return _context.SystemProfiles.Any(e => e.Id == id);
        }
    }
}
