using System.ComponentModel.DataAnnotations;

namespace EmployeesSysytem.Models.ViewModels
{
    public class HolidayViewModel:UserActivity
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        [Display(Name = "Start Date")]
        [DisplayFormat(ApplyFormatInEditMode =true,DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime StartTime { get; set; }
        [Display(Name = "End Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime EndTime { get; set; }
        public string? Description { get; set; }
        public Holiday Holiday { get; set; }
        public List<Holiday> Holidays { get; set; }
    }
}
