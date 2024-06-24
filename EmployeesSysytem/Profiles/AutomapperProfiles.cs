using AutoMapper;
using EmployeesSysytem.Models;
using EmployeesSysytem.Models.ViewModels;

namespace EmployeesSysytem.Profiles
{
    public class AutomapperProfiles:Profile
    {
        public AutomapperProfiles()
        {
            CreateMap<Employee, EmployeeViewModel>().ReverseMap();
        }
    }
}
