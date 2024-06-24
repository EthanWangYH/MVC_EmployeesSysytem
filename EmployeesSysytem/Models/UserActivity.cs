namespace EmployeesSysytem.Models
{
    public class UserActivity
    {
        public string? CreatedById { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? ModifiedById { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
    public class ApprovalActivity : UserActivity
    {
        public string? ApprovalById { get; set; }
        public DateTime ApprovalOn { get; set; }

    }
}


