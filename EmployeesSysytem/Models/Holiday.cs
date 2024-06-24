using System.ComponentModel.DataAnnotations;

namespace EmployeesSysytem.Models
{
    public class Holiday:UserActivity
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        [Display(Name ="Start Date")]
        [DisplayFormat(ApplyFormatInEditMode =true,DataFormatString ="{0:MM/dd/yyyy}")]
        public DateTime StartTime { get; set; }
        [Display(Name = "End Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime EndTime { get; set; }
        public string? Description { get; set; }
    }
}
