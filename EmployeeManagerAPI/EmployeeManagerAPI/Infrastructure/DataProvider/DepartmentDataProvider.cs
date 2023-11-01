using EmployeeManagerAPI.Infrastructure.Interfaces;
using EmployeeManagerAPI.Models;
using static EmployeeManagerAPI.Infrastructure.Models.Database;

namespace EmployeeManagerAPI.Helpers
{
    public class DepartmentDataProvider : IDepartmentDataProvider
    {
        private readonly IDataProvider _dataProvider;

        public DepartmentDataProvider(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async Task<IEnumerable<Department>> GetDepartments(wpsp_Departments_Select parameters)
        {
            IEnumerable<Department> result = await _dataProvider.ExecuteReaderCommandAsync<Department>("wpsp_Departments_Select", parameters);
            return result;
        }

        public async Task<Department> GetDepartment(wpsp_Departments_Select parameters)
        {
            IEnumerable<Department> result = await _dataProvider.ExecuteReaderCommandAsync<Department>("wpsp_Departments_Select", parameters);
            return result.FirstOrDefault();
        }

        public async void SaveDepartment(wpsp_Department_Save parameters)
        {
            await _dataProvider.ExecuteNonQueryCommandAsync("wpsp_Department_Save", parameters);
        }

        public async void DeleteDepartment(wpsp_Department_Delete parameters)
        {
            await _dataProvider.ExecuteNonQueryCommandAsync("wpsp_Department_Delete", parameters);
        }
    }
}
