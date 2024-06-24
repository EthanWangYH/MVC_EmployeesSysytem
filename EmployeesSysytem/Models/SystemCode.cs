using System.ComponentModel.DataAnnotations;

namespace EmployeesSysytem.Models
{
    public class SystemCode : UserActivity
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string Description { get; set; } = "";
    }

}
