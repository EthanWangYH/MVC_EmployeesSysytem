using EmployeesSysytem.Data;
using EmployeesSysytem.Models;
using EmployeesSysytem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EmployeesSysytem.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = dbContext;
        }
        public async Task<IActionResult> Details(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(y => y.Id == id);

            if (user == null)
            {
                return NotFound();
            }
            else
            {
                var roles = await _userManager.GetRolesAsync(user);
                var roleName = roles.FirstOrDefault();
                var model = new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    NationalId = user.NationalId,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber!,
                    RoleId = roleName
                };
                return View(model);
            }
        }
        public async Task<IActionResult> Index()
        {
            var users = await _context.ApplicationUsers.ToListAsync();
            var model = new List<UserViewModel>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var roleName = roles.FirstOrDefault();
                if (roleName == null)
                {
                    roleName = "No Role Assigned";
                }
                var role = await _roleManager.FindByNameAsync(roleName);
                model.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user?.UserName!,
                    Email = user?.Email,
                    FirstName = user?.FirstName,
                    MiddleName = user?.MiddleName,
                    LastName = user?.LastName,
                    PhoneNumber = user?.PhoneNumber!,
                    NationalId = user?.NationalId,
                    RoleId = role?.Name
                });
            }
            return View(model);
        }

        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
            return View(new UserViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
            if (model.ConfirmPassword != model.Password)
            {
                ModelState.AddModelError("Password", "ConfirmPassword Can't Match Password");
            }
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                FirstName = model.FirstName!,
                MiddleName = model.MiddleName!,
                LastName = model.LastName!,
                NationalId = model.NationalId!,
                Email = model.Email,
                NormalizedUserName = model.UserName,
                EmailConfirmed = true,
                PhoneNumber = model.PhoneNumber,
                PhoneNumberConfirmed = true,
                CreatedOn = DateTime.Now,
                CreatedById = "Ethan Wang",
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (model.RoleId != null)
            {
                var role = await _roleManager.FindByIdAsync(model.RoleId);

                if (result.Succeeded)
                {
                    if (model.RoleId != null && role != null && role.Name != null)
                    {
                        await _userManager.AddToRoleAsync(user, role.Name);
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(model);
                }
            }
            return View(model);
        }
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var roleId = userRoles.Any() ? (await _roleManager.FindByNameAsync(userRoles.First()))!.Id : null;

            var model = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber!,
                NationalId = user.NationalId,
                RoleId = roleId
            };

            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string id, UserViewModel model)
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", model.RoleId);
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.FirstName = model.FirstName!;
            user.MiddleName = model.MiddleName!;
            user.LastName = model.LastName!;
            user.PhoneNumber = model.PhoneNumber!;
            user.NationalId = model.NationalId!;
            user.NormalizedUserName = model.UserName.ToUpper();
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Failed to update user.");
                return View(model);
            }
            else
            {
                var currentRoles =await _userManager.GetRolesAsync(user);
                var newRole = (await _roleManager.FindByIdAsync(model.RoleId!))?.Name;
                if (currentRoles != null)
                {
                    var removeRolesResult = await _userManager.RemoveFromRolesAsync(user,currentRoles);
                    if (!removeRolesResult.Succeeded)
                    {
                        ModelState.AddModelError("", "Failed to remove old roles.");
                        return View(model);
                    }
                    if (newRole != null)
                    {
                        var addNewRolesResult = await _userManager.AddToRoleAsync(user, newRole);
                        if (!addNewRolesResult.Succeeded)
                        {
                            ModelState.AddModelError("", "Failed to add new role.");
                            return View(model);
                        }
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(model);
        }
    }
}
