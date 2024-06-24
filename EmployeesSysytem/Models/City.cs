using System.ComponentModel.DataAnnotations;

namespace EmployeesSysytem.Models
{
    public class City
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        [Display(Name="Country Name")]
        public int? CountryId { get; set; }
        public Country? Country { get; set; }
    }

}
