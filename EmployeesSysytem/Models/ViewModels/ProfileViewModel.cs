using System.ComponentModel.DataAnnotations;

namespace EmployeesSysytem.Models.ViewModels
{
    public class ProfileViewModel
    {
        public ICollection<SystemProfile> Profiles { get; set; }
        [Display(Name = "Role")]
        public string RoleId { get; set; }
        [Display(Name = "System Task")]
        public int TaskId { get; set; }
        public ICollection<int> RolesProfilesIds { get; set; }
        public int[] Ids { get; set; }

    }
}
