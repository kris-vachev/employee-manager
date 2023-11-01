using System.ComponentModel.DataAnnotations;

namespace EmployeeManagerAPI.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
