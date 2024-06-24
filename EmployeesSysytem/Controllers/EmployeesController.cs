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
using AutoMapper;
using EmployeesSysytem.Models.ViewModels;

namespace EmployeesSysytem.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public EmployeesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment,IMapper mapper)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            EmployeeViewModel employee = new EmployeeViewModel();
            employee.Employees =await _context.Employees!.
                Include(e => e.City).
                Include(e => e.Country).
                Include(e => e.Department).
                Include(e => e.Designation).
                Include(e => e.Gender).Include(e => e.Bank)
                .ToListAsync();
            return View(employee);
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees!
                .Include(e => e.City)
                .Include(e => e.Country)
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Gender)
                .Include(e => e.Bank)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name");
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name");
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            ViewData["DesignationId"] = new SelectList(_context.Designations, "Id", "Name");
            ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Name");
            ViewData["GenderId"] = new SelectList(_context.SystemCodeDetails!.Include(x => x.SystemCode).Where(y => y.SystemCode!.Name == "Gender"), "Id", "Code");
            ViewData["EmploymentTerm"] = new SelectList(Enum.GetValues(typeof(EmploymentTerm)));
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeViewModel newEmployee)
        {
            var employee = new Employee();
            var userId = User.FindFirstValue(ClaimTypes.Name);
            _mapper.Map(newEmployee, employee);
            employee.CreatedOn = DateTime.Now;
            employee.CreatedById = userId;
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (employee.ImageUpload != null)
                {
                    var imageName = DateTime.Now.ToString("MMM-dd-yyyy") + "_" + employee.ImageUpload.FileName;
                    var uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "Images/employees");
                    var filePath = Path.Combine(uploadDir, imageName);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await employee.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    employee.Image = imageName;
                }
                _context.Add(employee);
                await _context.SaveChangesAsync(userId);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", employee.CityId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", employee.CountryId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            ViewData["DesignationId"] = new SelectList(_context.Designations, "Id", "Name", employee.DesignationId);
            ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Name");
            ViewData["GenderId"] = new SelectList(_context.SystemCodeDetails!.Include(x => x.SystemCode).Where(y => y.SystemCode!.Name == "Gender"), "Id", "Code", employee.GenderId);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            EmployeeViewModel employeeModel = new EmployeeViewModel();
            var  employee = await _context.Employees!.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            _mapper.Map(employee, employeeModel);
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", employee.CityId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", employee.CountryId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            ViewData["DesignationId"] = new SelectList(_context.Designations, "Id", "Name", employee.DesignationId);
            ViewData["GenderId"] = new SelectList(_context.SystemCodeDetails!.Include(x => x.SystemCode).Where(y => y.SystemCode!.Name == "Gender"), "Id", "Code", employee.GenderId);
            ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Name");
            ViewData["EmploymentTerm"] = new SelectList(Enum.GetValues(typeof(EmploymentTerm)));
            ViewData["EmployeeStatus"] = new SelectList(Enum.GetValues(typeof(EmployeeStatus)));
            ViewData["TerminationReason"] = new SelectList(Enum.GetValues(typeof(TerminationReason)));
            ViewData["InactivityReason"] = new SelectList(Enum.GetValues(typeof(InactivityReason)));
            return View(employeeModel);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.Name);
                    var existingEmp = await _context.Employees!.FindAsync(id);
                    if (existingEmp != null)
                    {
                        var originalImage = existingEmp.Image;
                        var originalCreatedById = existingEmp.CreatedById;
                        var originalCreatedOn = existingEmp.CreatedOn;
                        // Update specific properties of existingEmp
                        _mapper.Map(model, existingEmp);
                        if (model.ImageUpload is not null)
                        {                  
                                if (originalImage is not null)
                                {
                                    var oldDir = Path.Combine(_webHostEnvironment.WebRootPath, "Images/employees");
                                    var oldPath = Path.Combine(oldDir, originalImage);
                                    if (System.IO.File.Exists(oldPath))
                                    {
                                        System.IO.File.Delete(oldPath);
                                    }
                                }
                            
                            var imageName = DateTime.Now.ToString("MMM-dd-yyyy") + "_" + model.ImageUpload.FileName;
                            var uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "Images/employees");
                            var filePath = Path.Combine(uploadDir, imageName);
                            FileStream fs = new FileStream(filePath, FileMode.Create);
                            await model.ImageUpload.CopyToAsync(fs);
                            fs.Close();
                            existingEmp.Image = imageName;
                            _context.Entry(existingEmp).Property(x => x.Image).IsModified = true;
                        }
                        else
                        {
                            _context.Entry(existingEmp).Property(x => x.Image).IsModified = false;
                        }
                        // Set modified properties
                        existingEmp.ModifiedById = userId;
                        existingEmp.ModifiedOn = DateTime.Now;
                        existingEmp.CreatedOn = originalCreatedOn;
                        existingEmp.CreatedById = originalCreatedById;
                        _context.Entry(existingEmp).Property(x => x.CreatedById).IsModified = false;
                        _context.Entry(existingEmp).Property(x => x.CreatedOn).IsModified = false;
                       // Save changes to the database
                        await _context.SaveChangesAsync(userId);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(model.Id))
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
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", model.CityId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", model.CountryId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", model.DepartmentId);
            ViewData["DesignationId"] = new SelectList(_context.Designations, "Id", "Name", model.DesignationId);
            ViewData["GenderId"] = new SelectList(_context.SystemCodeDetails!.Include(x => x.SystemCode).Where(y => y.SystemCode!.Name == "Gender"), "Id", "Code", model.GenderId);
            ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Name");
            ViewData["EmploymentTerm"] = new SelectList(Enum.GetValues(typeof(EmploymentTerm)));
            ViewData["EmployeeStatus"] = new SelectList(Enum.GetValues(typeof(EmployeeStatus)));
            ViewData["TerminationReason"] = new SelectList(Enum.GetValues(typeof(TerminationReason)));
            ViewData["InactivityReason"] = new SelectList(Enum.GetValues(typeof(InactivityReason)));
            return View(model);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees!
                .Include(e => e.City)
                .Include(e => e.Country)
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Gender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.Name);
            var employee = await _context.Employees!.FindAsync(id);
            if (employee != null)
            {
                if (!string.Equals(employee.Image, "NoImage.png"))
                {
                    var uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "Images/producer");
                    var oldPath = Path.Combine(uploadDir, employee.Image!);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees!.Any(e => e.Id == id);
        }
    }
}
