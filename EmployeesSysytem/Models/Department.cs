﻿using System.ComponentModel.DataAnnotations;

namespace EmployeesSysytem.Models
{
    public class Department : UserActivity
    {
        [Key]
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
    }

}