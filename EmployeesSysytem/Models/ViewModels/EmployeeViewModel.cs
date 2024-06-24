using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EmployeesSysytem.Models.ViewModels
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }

        public string EmployeeNumber { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        [Display(Name = "Employee")]
        public string FullName => $"{FirstName} {MiddleName} {LastName}";
        public string? MiddleName { get; set; }
        [Display(Name = "Email")]
        public string? EmailAddress { get; set; }
        [Display(Name = "City")]
        public int? CityId { get; set; }
        [Display(Name = "Country")]
        public int? CountryId { get; set; }
        [Display(Name = "Birth Of Date")]
        public DateTime? BirthOfDate { get; set; }
        public string? Address { get; set; }
        [Display(Name = "Department")]
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
        [Display(Name = "Designation")]
        public int? DesignationId { get; set; }
        [Display(Name = "Gender")]
        public int? GenderId { get; set; }
        [Display(Name = "Employment Date")]
        public DateTime? EmploymentDate { get; set; }
        [Display(Name = "Status")]
        public EmployeeStatus EmployeeStatus { get; set; } = EmployeeStatus.Active;
        public DateTime? InactiveDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        [Display(Name = "Termination Reason")]
        public TerminationReason? TerminationReason { get; set; }
        [Display(Name = "Inactivity Reason")]
        public InactivityReason? InactivityReason { get; set; }
        [Display(Name = "Bank")]
        public int? BankId { get; set; }
        [Display(Name = "Account No.")]
        public string? AccountNumber { get; set; }
        public int SSN { get; set; }
        [Display(Name = "Employment Term")]
        public EmploymentTerm EmploymentTerm { get; set; }
        [Display(Name = "Allocated Leave")]
        public decimal AllocatedLeaveDays { get; set; } = 30;
        public decimal TotalLeaveDays { get; set; }
        [Display(Name = "Leave OutStanding Balance")]
        public decimal? LeaveOutStandingBalance { get; set; }
        [Display(Name = "Employee Photo")]
        public string? Image { get; set; }
        [NotMapped]
        public IFormFile? ImageUpload { get; set; }
        public Employee Employee { get; set; }
        public List<Employee> Employees { get; set;}
        public string UserId { get; set; }
        [Display(Name = "Disability No.")]
        public string? DisabilityId { get; set; }
    }
}
