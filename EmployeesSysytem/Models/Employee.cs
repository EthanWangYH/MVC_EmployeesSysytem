using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeesSysytem.Models
{
    public class Employee : UserActivity
    {
        public int Id { get; set; }

        public string EmployeeNumber { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        [Display(Name = "Employee")]
        public string FullName => $"{FirstName} {MiddleName} {LastName}";
        public string? MiddleName { get; set; }
        [Display(Name ="Email")]
        public string? EmailAddress { get; set; }
        [Display(Name ="City")]
        public int? CityId { get; set; }
        public City? City { get; set; }
        [Display(Name ="Country")]
        public int? CountryId { get; set; }
        public Country? Country { get; set; }
        [Display(Name ="Birth Of Date")]
        public DateTime? BirthOfDate { get; set; }
        public string? Address { get; set; }
        [Display(Name ="Department")]
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
        [Display(Name = "Designation")]
        public int? DesignationId { get; set; }
        public Designation? Designation { get; set; }
        [Display(Name ="Gender")]
        public int? GenderId { get; set; }
        public SystemCodeDetail? Gender { get; set; }
        [Display(Name = "Employment Date")]
        public DateTime? EmploymentDate { get; set; }
        [Display(Name ="Status")]
        public EmployeeStatus EmployeeStatus { get; set; }=EmployeeStatus.Active;
        public DateTime? InactiveDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        [Display(Name = "Termination Reason")]
        public TerminationReason? TerminationReason { get; set; }
        [Display(Name = "Inactivity Reason")]
        public InactivityReason? InactivityReason { get; set; }
        [Display(Name = "Bank")]
        public int? BankId { get; set; }
        public Bank? Bank { get; set; }
        [Display(Name ="Account No.")]
        public string? AccountNumber { get; set; }
        public int SSN { get; set; }
        [Display(Name = "Employment Term")]
        public EmploymentTerm EmploymentTerm { get; set; }
        [Display(Name = "Allocated Leave")]
        public decimal AllocatedLeaveDays { get; set; } = 30;
        public decimal TotalLeaveDays { get; set; }
        [Display(Name = "Leave OutStanding Balance")]
        public decimal? LeaveOutStandingBalance { get; set; }
        [Display(Name ="Employee Photo")]  
        public string? Image {  get; set; }
        public string UserId { get; set; }
        public string? DisabilityId { get; set; }
        [NotMapped]
        public IFormFile? ImageUpload { get; set; }

        
    }
    public enum EmployeeStatus
    {
        Active=0,
        Inactive=1,
    }
    public enum InactivityReason
    {
        [Display(Name = "Resigned")]
        Resigned,
        [Display(Name = "Terminated")]
        Terminated,
        [Display(Name = "On Leave")]
        OnLeave,
        [Display(Name = "Other")]
        Other
    }
    public enum TerminationReason
    {
        [Display(Name = "Poor Performance")]
        PoorPerformance,
        [Display(Name = "Violation of Company Policy")]
        ViolationOfCompanyPolicy,
        [Display(Name = "Misconduct")]
        Misconduct,
        [Display(Name = "Attitude Problems")]
        AttitudeProblems,
        [Display(Name = "Absenteeism and Tardiness")]
        AbsenteeismAndTardiness,
        [Display(Name = "Redundancy/Downsizing")]
        RedundancyDownsizing,
        [Display(Name = "Position Elimination")]
        PositionElimination,
        [Display(Name = "Legal Violations")]
        LegalViolations,
        [Display(Name = "Conflict with Colleagues or Management")]
        ConflictWithColleaguesOrManagement,
        [Display(Name = "Lack of Capability")]
        LackOfCapability,
        [Display(Name = "End of Contract")]
        EndOfContract,
        [Display(Name = "Retirement")]
        Retirement,
        [Display(Name = "Voluntary Resignation")]
        VoluntaryResignation,
        [Display(Name = "Health Issues")]
        HealthIssues,
        [Display(Name = "Other")]
        Other
    }
    public enum EmploymentTerm
    {  
        Temporary,
        Contract,
        Permanent
    }
}
