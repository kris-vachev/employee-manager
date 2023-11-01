using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using EmployeeManagerAPI.Interfaces;

namespace EmployeeManagerAPI.Models
{
    public class Employee : IPagination
    {
        [Key]
        public int EmployeeId { get; set; }
        [Required]
        [ForeignKey("Departments")]
        public int DepartmentId { get; set; }

        [Required]
        public string EmployeeName { get; set; }
        [Required]
        public int Salary { get; set; }
        [Required]
        public DateTime DateJoined { get; set; }

        // Relations
        public Department? Department { get; set; }

        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public int? TotalCount { get; set; }
    }
}
