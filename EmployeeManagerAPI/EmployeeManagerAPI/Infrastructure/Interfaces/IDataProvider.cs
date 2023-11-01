using EmployeeManagerAPI.Models;
using static EmployeeManagerAPI.Infrastructure.Models.Database;

namespace EmployeeManagerAPI.Infrastructure.Interfaces
{
    public interface IDataProvider
    {
        Task<IEnumerable<T>> ExecuteReaderCommandAsync<T>(string commandText, object parameters);
        Task ExecuteNonQueryCommandAsync(string commandText, object parameters);
    }
}
