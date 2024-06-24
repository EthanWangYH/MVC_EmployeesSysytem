using System.ComponentModel.DataAnnotations;

namespace EmployeesSysytem.Models
{
    public class LeaveApplication : ApprovalActivity
    {
        public int Id { get; set; }
        [Display(Name ="Employee")]
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        [Display(Name = "Days")]
        public int NoOfDays { get; set; }
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        [Display(Name ="Duration")]
        public int DurationId { get; set; }
        public SystemCodeDetail? Duration { get; set; }
        [Display(Name = "Leave Type")]
        public int LeaveTypeId { get; set; }
        public LeaveType? LeaveType { get; set; }
        public string? Attachment { get; set; }
        [Display(Name = "Notes")]
        public string? Description { get; set; }
        [Display(Name = "Status")]
        public LeaveStatus LeaveStatus { get; set; } = LeaveStatus.AwaitingApprove;
        public bool IsChanged { get; set; } = false;
    }

    public enum LeaveStatus
    {
        AwaitingApprove,
        Pending,
        Apprvoed,
        Rejected
    }

}
