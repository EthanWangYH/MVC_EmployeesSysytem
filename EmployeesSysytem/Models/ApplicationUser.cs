using Microsoft.AspNetCore.Identity;

namespace EmployeesSysytem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = "";
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = "";
        public string? NationalId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? CreatedById { get; set; }
        public string FullName => $"{FirstName} {MiddleName} {LastName}";
        public string? ModifiedById { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime? LoginDate { get; set; }
        public DateTime? PasswordChangeOn { get; set; }

    }
}
