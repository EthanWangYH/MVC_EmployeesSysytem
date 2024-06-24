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
using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Elfie.Model.Strings;
using EmployeesSysytem.Models.ViewModels;

namespace EmployeesSysytem.Controllers
{
    public class LeaveApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;

        public LeaveApplicationsController(ApplicationDbContext context, IWebHostEnvironment environment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _environment = environment;
            _userManager = userManager;
        }

        // GET: LeaveApplications
        public async Task<IActionResult> Index()
        {
            var applicationsQuery = _context.LeaveApplications!.Include(l => l.Duration)
            .Include(l => l.Employee)
            .Include(l => l.LeaveType)
            .AsQueryable();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = _context.Employees!.FirstOrDefault(x => x.UserId == userId);
            if (userId != null)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var role = (await GetRole()).ToString();
                    if (role != null)
                    {
                        if (role.Contains("manager"))
                        {
                            applicationsQuery = applicationsQuery.
                                Where(d => d.Employee!.DepartmentId == employee!.DepartmentId);

                        }
                        else if (role.Contains("boss") || role.Contains("admin") || role.Contains("hr"))
                        {

                        }
                        else
                        {
                            applicationsQuery = applicationsQuery.Where(x => x.Employee!.UserId == userId);
                        }

                    }
                }
            }
            var applications = await applicationsQuery.ToListAsync();
            return View(applications);
        }

        // GET: LeaveApplications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveApplication = await _context.LeaveApplications!
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveApplication == null)
            {
                return NotFound();
            }
            var userId = GetUserId();
            var role = (await GetRole()).ToString();

            if (role != null)
            {
                ViewBag.isManager = role.Contains("manager");
                ViewBag.isBoss = role.Contains("boss");
                ViewBag.isAdmin = role.Contains("admin");
                ViewBag.isHR = role.Contains("hr");
                ViewBag.isApplicationer = leaveApplication.Employee != null && leaveApplication.Employee.UserId == userId;
            }
            else
            {
                ViewBag.isManager = false;
                ViewBag.isBoss = false;
                ViewBag.isAdmin = false;
                ViewBag.isHR = false;
                ViewBag.isApplicationer = false;
            }
            ViewBag.leavedays = leaveApplication.NoOfDays;
            return View(leaveApplication);
        }

        public async Task<IActionResult> DownloadFile(int id)
        {
            var leaveApplication = await _context.LeaveApplications!.FindAsync(id);
            if (leaveApplication == null || string.IsNullOrEmpty(leaveApplication.Attachment))
            {
                return NotFound();
            }
            var pathDir = Path.Combine(_environment.WebRootPath, "LeaveApplications/Attachments");
            var filePath = Path.Combine(pathDir, leaveApplication.Attachment);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, leaveApplication.Attachment);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int? id, string status)
        {
            var userId = GetUserId();
            var user = _context.ApplicationUsers.FirstOrDefault(x => x.Id == userId);
            var leaveApplication = await _context.LeaveApplications!.Include(x => x.Employee).FirstOrDefaultAsync(y => y.Id == id);
            var employee = leaveApplication?.Employee;
            if (id == null || _context.LeaveApplications == null)
            {
                return NotFound();
            }
            if (status == "Apprvoed")
            {
                leaveApplication!.LeaveStatus = LeaveStatus.Apprvoed;
                leaveApplication!.ApprovalOn = DateTime.Now;
                leaveApplication.ApprovalById = user?.FullName ?? null;
                var leaveAdjust = new LeaveAdjustmentEntry()
                {
                    EmployeeId = employee!.Id,
                    NoOfDays = leaveApplication?.NoOfDays ?? 0,
                    LeaveAdjustmentDate = DateTime.Now,
                    LeaveStartDate = leaveApplication!.StartDate,
                    LeaveEndDate = leaveApplication!.EndDate
                };
                if (leaveApplication.LeaveTypeId == 1)
                {
                    employee.TotalLeaveDays = employee.TotalLeaveDays + leaveApplication.NoOfDays;
                    employee.LeaveOutStandingBalance = employee.AllocatedLeaveDays - employee.TotalLeaveDays;
                }
                leaveApplication.IsChanged = true;
                _context.LeaveApplications.Update(leaveApplication!);
                _context.Employees!.Update(employee);
                _context.LeaveAdjustmentEntries!.Add(leaveAdjust);
                await _context.SaveChangesAsync(userId);
                return RedirectToAction("Index");

            }
            else if (status == "Pending")
            {
                leaveApplication!.LeaveStatus = LeaveStatus.Pending;
            }
            else if (status == "Rejected")
            {
                leaveApplication!.LeaveStatus = LeaveStatus.Rejected;
                leaveApplication.IsChanged = true;
            }
            _context.LeaveApplications.Update(leaveApplication!);
            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Details), new { id = leaveApplication!.Id });
        }


        // GET: LeaveApplications/Create
        public IActionResult Create()
        {
            ViewData["DurationId"] = new SelectList(_context.SystemCodeDetails!.Include(x => x.SystemCode).Where(y => y.SystemCode!.Description == "Duration"), "Id", "Description");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name");
            return View();
        }

        // POST: LeaveApplications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveApplication leaveApplication, IFormFile leaveAttachment)
        {
            ViewData["DurationId"] = new SelectList(_context.SystemCodeDetails!.Include(x => x.SystemCode).Where(y => y.SystemCode!.Name == "Duration"), "Id", "Description");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", leaveApplication.EmployeeId);
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name", leaveApplication.LeaveTypeId);
            try
            {
                if (leaveAttachment != null && leaveAttachment.Length > 0)
                {
                    var fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + leaveAttachment.FileName;
                    var uploadDir = Path.Combine(_environment.WebRootPath, "LeaveApplications/Attachments");
                    var filePath = Path.Combine(uploadDir, fileName);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await leaveAttachment.CopyToAsync(fs);
                    fs.Close();
                    leaveApplication.Attachment = fileName;
                }
                var userId = User.FindFirstValue(ClaimTypes.Name);
                leaveApplication.CreatedOn = DateTime.Now;
                leaveApplication.CreatedById = "Ethan Wang";
                _context.Add(leaveApplication);
                await _context.SaveChangesAsync(userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View(leaveApplication);

            }
        }

        // GET: LeaveApplications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveApplication = await _context.LeaveApplications!.FindAsync(id);
            if (leaveApplication == null)
            {
                return NotFound();
            }
            ViewData["DurationId"] = new SelectList(_context.SystemCodeDetails!.Include(x => x.SystemCode).Where(y => y.SystemCode!.Name == "Duration"), "Id", "Description");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", leaveApplication.EmployeeId);
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name", leaveApplication.LeaveTypeId);
            return View(leaveApplication);
        }

        // POST: LeaveApplications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LeaveApplication leaveApplication, IFormFile leaveAttachment)
        {
            ViewData["DurationId"] = new SelectList(_context.SystemCodeDetails!.Include(x => x.SystemCode).Where(y => y.SystemCode!.Name == "Duration"), "Id", "Description");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name");

            try
            {
                if (id != leaveApplication.Id)
                {
                    return NotFound();
                }
                var userId = GetUserId();
                var existingLeaveApp = await _context.LeaveApplications!.FindAsync(id);
                if (existingLeaveApp != null)
                {
                    existingLeaveApp.LeaveTypeId = leaveApplication.LeaveTypeId;
                    existingLeaveApp.EndDate = leaveApplication.EndDate;
                    existingLeaveApp.StartDate = leaveApplication.StartDate;
                    existingLeaveApp.DurationId = leaveApplication.DurationId;
                    existingLeaveApp.Description = leaveApplication.Description;
                    existingLeaveApp.LeaveStatus = LeaveStatus.AwaitingApprove;
                    existingLeaveApp.EmployeeId = leaveApplication.EmployeeId;
                    existingLeaveApp.NoOfDays = leaveApplication.NoOfDays;
                    leaveApplication.ModifiedById = userId;
                    leaveApplication.ModifiedOn = DateTime.Now;
                    if (leaveAttachment.Length > 0)
                    {
                        if (existingLeaveApp.Attachment != null)
                        {
                            var oldDir = Path.Combine(_environment.WebRootPath, "LeaveApplications/Attachments");
                            if (existingLeaveApp.Attachment != null)
                            {
                                var oldPath = Path.Combine(oldDir, existingLeaveApp.Attachment);
                                if (System.IO.File.Exists(oldPath))
                                {
                                    System.IO.File.Delete(oldPath);
                                }
                            }
                        }
                        var fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + leaveAttachment.FileName;
                        var uploadDir = Path.Combine(_environment.WebRootPath, "LeaveApplications/Attachments");
                        var filePath = Path.Combine(uploadDir, fileName);
                        FileStream fs = new FileStream(filePath, FileMode.Create);
                        await leaveAttachment.CopyToAsync(fs);
                        fs.Close();
                        existingLeaveApp.Attachment = fileName;
                        _context.Entry(existingLeaveApp).Property(x => x.Attachment).IsModified = true;
                    }
                    else
                    {
                        _context.Entry(existingLeaveApp).Property(x => x.Attachment).IsModified = false;
                    }
                    _context.Entry(existingLeaveApp).Property(x => x.CreatedById).IsModified = false;
                    _context.Entry(existingLeaveApp).Property(x => x.CreatedOn).IsModified = false;

                    await _context.SaveChangesAsync(userId);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                if (!LeaveApplicationExists(leaveApplication.Id))
                {
                    return NotFound();
                }
                else
                {
                    return View(leaveApplication);
                }
            }
        }

        // GET: LeaveApplications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveApplication = await _context.LeaveApplications!
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (leaveApplication == null)
            {
                return NotFound();
            }

            return View(leaveApplication);
        }

        // POST: LeaveApplications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetUserId();
            var leaveApplication = await _context.LeaveApplications!.FindAsync(id);
            if (leaveApplication != null)
            {
                if (leaveApplication.Attachment != null)
                {
                    var oldDir = Path.Combine(_environment.WebRootPath, "LeaveApplications/Attachments");
                    var oldPath = Path.Combine(oldDir, leaveApplication.Attachment);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }

                }
                _context.LeaveApplications.Remove(leaveApplication);
            }

            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));
        }
       
        private bool LeaveApplicationExists(int id)
        {
            return _context.LeaveApplications!.Any(e => e.Id == id);
        }
        private string GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                return userId;
            }
            else
            {
                return string.Empty;
            }
        }
        private async Task<string> GetRole()
        {
            var userId = GetUserId();
            if (userId != null)
            {
                var user = await _context.ApplicationUsers.FindAsync(userId);
                if (user != null)
                {
                    var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                    if (role != null)
                    {
                        role = role.ToLower();
                        return role;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
