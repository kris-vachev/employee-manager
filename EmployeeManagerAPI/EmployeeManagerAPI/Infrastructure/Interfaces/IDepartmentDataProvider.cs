using EmployeeManagerAPI.Models;
using static EmployeeManagerAPI.Infrastructure.Models.Database;

namespace EmployeeManagerAPI.Infrastructure.Interfaces
{
    public interface IDepartmentDataProvider
    {
        Task<IEnumerable<Department>> GetDepartments(wpsp_Departments_Select parameters);
        Task<Department> GetDepartment(wpsp_Departments_Select parameters);
        void SaveDepartment(wpsp_Department_Save parameters);
        void DeleteDepartment(wpsp_Department_Delete parameters);
    }
}
