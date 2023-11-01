using EmployeeManagerAPI.Models;
using static EmployeeManagerAPI.Infrastructure.Models.Database;

namespace EmployeeManagerAPI.Infrastructure.Interfaces
{
    public interface IUserDataProvider
    {
        Task<User> GetUser(wpsp_User_Select select);
    }
}
