using EmployeesSysytem.Data;
using EmployeesSysytem.Models;
using EmployeesSysytem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace EmployeesSysytem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var applicationsQuery = _context.LeaveApplications!.Include(l => l.Duration)
               .Include(l => l.Employee)
               .AsQueryable();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees!.FirstOrDefaultAsync(x => x.UserId == userId);

            int myChanged = 0;
            int awaitingApproval = 0;
            string changedMessage = "no leave application status changed";
            string approvalMessage = "no leave application awaiting approval";

            if (employee != null)
            {
                var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()?.ToLower();

                if (role != null)
                {
                    if (role.Contains("boss"))
                    {
                        var awaitingApprovalApplication = applicationsQuery
                            .Where(x => x.LeaveStatus == LeaveStatus.AwaitingApprove || x.LeaveStatus == LeaveStatus.Pending)
                            .ToList();
                        awaitingApproval = awaitingApprovalApplication.Count;
                        approvalMessage = awaitingApproval == 1 ? "1 leave application awaiting approval" :
                            $"{awaitingApproval} leave applications awaiting approval";
                    }
                    else if (role.Contains("manager"))
                    {
                        var awaitingApprovalApplication = applicationsQuery
                            .Where(x => x.LeaveStatus == LeaveStatus.AwaitingApprove && x.Employee!.DepartmentId == employee.Id)
                            .ToList();
                        awaitingApproval = awaitingApprovalApplication.Count;
                        approvalMessage = awaitingApproval == 1 ? "1 application awaiting approval" :
                            $"{awaitingApproval} applications awaiting approval";

                        var mychangedApplication = applicationsQuery.Where(x => x.Employee!.UserId == userId).ToList();
                        myChanged = mychangedApplication.Count;
                        changedMessage = mychangedApplication.Count == 1 ? "1 application status changed" :
                            $"{mychangedApplication.Count} applications status changed";
                    }
                    else if (role.Contains("hr"))
                    {
                        var mychangedApplication = applicationsQuery.ToList();
                        myChanged = mychangedApplication.Count;
                        changedMessage = mychangedApplication.Count == 1 ? "1 leave application status changed" :
                            $"{mychangedApplication.Count} applications status changed";
                    }
                    else
                    {
                        var mychangedApplication = applicationsQuery.Where(x => x.Employee!.UserId == userId).ToList();
                        myChanged = mychangedApplication.Count;
                        changedMessage = mychangedApplication.Count == 1 ? "1 application status changed" :
                            $"{mychangedApplication.Count} applications status changed";
                    }
                }
            }
            HttpContext.Session.SetInt32("MyChanged", myChanged);
            HttpContext.Session.SetString("ChangedMessage", changedMessage);
            HttpContext.Session.SetString("ApprovalMessage", approvalMessage);
            HttpContext.Session.SetInt32("AwaitingApproval", awaitingApproval);
            HttpContext.Session.SetInt32("TotalNotifications", myChanged + awaitingApproval);
            ViewData["MyChanged"] = myChanged;
            ViewData["ChangedMessage"] = changedMessage;
            ViewData["ApprovalMessage"] = approvalMessage;
            ViewData["AwaitingApproval"] = awaitingApproval;
            ViewData["TotalNotifications"] = myChanged + awaitingApproval;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> MarkNotificationsAsRead()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var myChangedApplications = await _context.LeaveApplications!
                .Where(x => x.Employee!.UserId == userId && x.IsChanged).ToListAsync();

            if (myChangedApplications != null)
            {
                foreach (var application in myChangedApplications)
                {
                    application.IsChanged = false;
                }
                await _context.SaveChangesAsync();
            }
            UpdateSessionNotificationsCount(userId!);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> MarkSpecificNotificationAsRead(string type)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == userId);

            var applicationsToUpdate = new List<LeaveApplication>();

            if (type == "approval")
            {
                applicationsToUpdate = await _context.LeaveApplications!
                    .Where(x => x.Employee!.UserId == userId && (x.LeaveStatus == LeaveStatus.AwaitingApprove || x.LeaveStatus == LeaveStatus.Pending))
                    .ToListAsync();
            }
            else if (type == "change")
            {
                applicationsToUpdate = await _context.LeaveApplications!
                    .Where(x => x.Employee!.UserId == userId && x.IsChanged)
                    .ToListAsync();
            }

            if (applicationsToUpdate != null)
            {
                foreach (var application in applicationsToUpdate)
                {
                    application.IsChanged = false;
                }

                await _context.SaveChangesAsync();
            }

            // 更新会话中的通知数量
            UpdateSessionNotificationsCount(userId!);

            return Ok();
        }
        private void UpdateSessionNotificationsCount(string userId)
        {
            var myChanged = _context.LeaveApplications!
                .Count(x => x.Employee!.UserId == userId && x.IsChanged);

            HttpContext.Session.SetInt32("MyChanged", myChanged);
            HttpContext.Session.SetString("ChangedMessage", myChanged == 1 ? "1 application status changed" : $"{myChanged} applications status changed");

            var awaitingApproval = _context.LeaveApplications!
                .Count(x => x.Employee!.UserId == userId && (x.LeaveStatus == LeaveStatus.AwaitingApprove || x.LeaveStatus == LeaveStatus.Pending));

            HttpContext.Session.SetInt32("AwaitingApproval", awaitingApproval);
            HttpContext.Session.SetString("ApprovalMessage", awaitingApproval == 1 ? "1 application awaiting approval" : $"{awaitingApproval} applications awaiting approval");

            var totalNotifications = myChanged + awaitingApproval;
            HttpContext.Session.SetInt32("TotalNotifications", totalNotifications);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
