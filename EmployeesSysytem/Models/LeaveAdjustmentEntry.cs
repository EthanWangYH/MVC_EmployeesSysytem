namespace EmployeesSysytem.Models
{
    public class LeaveAdjustmentEntry
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public decimal? NoOfDays { get; set; }
        public string LeavePeriod { get; set; } = DateTime.Now.Year.ToString();
        public DateTime LeaveAdjustmentDate { get; set; }
        public DateTime LeaveStartDate { get; set; }
        public DateTime LeaveEndDate { get; set;}
    }
}
