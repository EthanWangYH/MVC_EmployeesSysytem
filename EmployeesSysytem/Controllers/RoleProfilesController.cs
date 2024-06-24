using EmployeesSysytem.Data;
using EmployeesSysytem.Models;
using EmployeesSysytem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace EmployeesSysytem.Controllers
{
    public class RoleProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public RoleProfilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public  async Task<IActionResult> Index()
        {
            var task = new ProfileViewModel();
            var roles = _context.Roles.ToList();
            var profiles = await _context.SystemProfiles.
                Include(x => x.Profile).
                ThenInclude(y => y.Children).
                ThenInclude(z => z.Children).
                ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            ViewBag.Tasks =new SelectList(profiles, "Id", "Name");
            return View(task);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRights(ProfileViewModel model)
        {
            var role = new RoleProfile()
            {
                TaskId = model.TaskId,
                RoleId = model.RoleId
            };
            var userId = User.FindFirstValue(ClaimTypes.Name);
            _context.RoleProfiles.Add(role);
            await _context.SaveChangesAsync(userId);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UserRights(string id)
        {
            var roleProfiles =await _context.RoleProfiles
                .Include(x=>x.Task)
                .Where(y=>y.RoleId==id)
                .ToListAsync();
            var model = new ProfileViewModel()
            {
                RoleId = id,
                Profiles = await _context.SystemProfiles.ToListAsync(),
                RolesProfilesIds = roleProfiles.Select(x=>x.TaskId).ToList(),
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> UserRights(ProfileViewModel viewModel)
        {
            if (viewModel.Ids != null && viewModel.Ids.Any())
            {
                var existingRoleProfiles = await _context.RoleProfiles
                    .Where(rp => rp.RoleId == viewModel.RoleId)
                    .ToListAsync();
                _context.RemoveRange(existingRoleProfiles);
                foreach(var taskId in viewModel.Ids)
                {
                    var profile = new RoleProfile()
                    {
                        TaskId = taskId,
                        RoleId = viewModel.RoleId,
                    };
                    _context.RoleProfiles.Add(profile);
                }
              

                var userId = User.FindFirstValue(ClaimTypes.Name);
                await _context.SaveChangesAsync(userId);
            }
            return RedirectToAction("Index", "Roles");
        }

    }
}

