using EmployeeManagerAPI.Infrastructure.Interfaces;
using EmployeeManagerAPI.Models;
using static EmployeeManagerAPI.Infrastructure.Models.Database;

namespace EmployeeManagerAPI.Helpers
{
    public class UserDataProvider : IUserDataProvider
    {
        private readonly IDataProvider _dataProvider;

        public UserDataProvider(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async Task<User> GetUser(wpsp_User_Select parameters)
        {
            IEnumerable<User> result = await _dataProvider.ExecuteReaderCommandAsync<User>("wpsp_User_Select", parameters);
            return result.FirstOrDefault();
        }
    }
}
