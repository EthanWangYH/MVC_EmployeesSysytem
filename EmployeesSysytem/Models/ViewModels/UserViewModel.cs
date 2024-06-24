using System.ComponentModel.DataAnnotations;

namespace EmployeesSysytem.Models.ViewModels
{
    public class UserViewModel:UserActivity
    {
        public string Id { get; set; }
        [Display(Name = "Email Address")]
        public string? Email { get; set; }
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }
        [Display(Name = "Middle Name")]
        public string? MiddleName { get; set; }
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }
        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {MiddleName} {LastName}";
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        public string? NationalId { get; set; }
        [Display(Name = "User Role")]
        public string? RoleId { get; set; }
        [Compare("Password", ErrorMessage = "Confirm Password Can't match Password")]
        public string ConfirmPassword { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public List<ApplicationUser> ApplicationUsers { get; set; }
    }

}
