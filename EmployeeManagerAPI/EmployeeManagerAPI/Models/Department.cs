using System.ComponentModel.DataAnnotations;
using EmployeeManagerAPI.Interfaces;

namespace EmployeeManagerAPI.Models
{
    public class Department : IPagination
    {
        [Key]
        public int? DepartmentId { get; set; }

        [Required]
        public string? DepartmentName { get; set; }

        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public int? TotalCount { get; set; }
    }
}
