using EmployeeManagerAPI.Infrastructure.Interfaces;
using EmployeeManagerAPI.Models;
using static EmployeeManagerAPI.Infrastructure.Models.Database;

namespace EmployeeManagerAPI.Helpers
{
    public class EmployeeDataProvider : IEmployeeDataProvider
    {
        private readonly IDataProvider _dataProvider;

        public EmployeeDataProvider(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async Task<IEnumerable<Employee>> GetEmployees(wpsp_Employees_Select parameters)
        {
            IEnumerable<Employee> result = await _dataProvider.ExecuteReaderCommandAsync<Employee>("wpsp_Employees_Select", parameters);
            return result;
        }

        public async Task<Employee> GetEmployee(wpsp_Employees_Select parameters)
        {
            IEnumerable<Employee> result = await _dataProvider.ExecuteReaderCommandAsync<Employee>("wpsp_Employees_Select", parameters);
            return result.FirstOrDefault();
        }

        public async void SaveEmployee(wpsp_Employee_Save parameters)
        {
            await _dataProvider.ExecuteNonQueryCommandAsync("wpsp_Employee_Save", parameters);
        }

        public async void DeleteEmployee(wpsp_Employee_Delete parameters)
        {
            await _dataProvider.ExecuteNonQueryCommandAsync("wpsp_Employee_Delete", parameters);
        }
    }
}
