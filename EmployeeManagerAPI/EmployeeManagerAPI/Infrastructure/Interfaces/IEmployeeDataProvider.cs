using EmployeeManagerAPI.Models;
using static EmployeeManagerAPI.Infrastructure.Models.Database;

namespace EmployeeManagerAPI.Infrastructure.Interfaces
{
    public interface IEmployeeDataProvider
    {
        Task<IEnumerable<Employee>> GetEmployees(wpsp_Employees_Select parameters);
        Task<Employee> GetEmployee(wpsp_Employees_Select parameters);
        void SaveEmployee(wpsp_Employee_Save parameters);
        void DeleteEmployee(wpsp_Employee_Delete parameters);
    }
}
