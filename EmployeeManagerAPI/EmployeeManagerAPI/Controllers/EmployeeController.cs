using EmployeeManagerAPI.Helpers;
using EmployeeManagerAPI.Infrastructure.Helpers;
using EmployeeManagerAPI.Infrastructure.Interfaces;
using EmployeeManagerAPI.Infrastructure.Models;
using EmployeeManagerAPI.Interfaces;
using EmployeeManagerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Data;
using static EmployeeManagerAPI.Infrastructure.Models.Database;

namespace EmployeeManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : BaseController
    {
        private readonly string _cacheKey = "EmployeeList";
        private readonly IEmployeeDataProvider _dataProvider;
        private readonly IOptions<CacheSettings> _cacheSettings;
        private ICacheManager _cacheManager;

        public EmployeeController(IEmployeeDataProvider dataProvider, IOptions<CacheSettings> cacheSettings, ICacheManager cacheManager)
        {
            _dataProvider = dataProvider;
            _cacheManager = cacheManager;
            _cacheSettings = cacheSettings;
        }

        [HttpGet]
        [Authorize(Roles = "User, Administrator")]
        public async Task<ActionResult> GetEmployees([FromQuery] APIRequest request)
        {
            // get response
            APIResponse<IEnumerable<Employee>> _response = new APIResponse<IEnumerable<Employee>>();

            try
            {
                // get cached value
                // - get cache key
                string cacheKey = GetCacheKey(_cacheKey, request);
                // - get cached value
                IEnumerable<Employee> employees = _cacheManager.Get<IEnumerable<Employee>>(cacheKey);
                if (employees != null)
                {
                    _response = new APIResponse<IEnumerable<Employee>>() { Status = true, Msg = "Ok", Value = employees };
                }
                else
                {
                    // get employees from DB
                    employees = await _dataProvider.GetEmployees(new wpsp_Employees_Select
                    {
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize,
                        Filter = request.FilterValue,
                        SortField = request.SortField,
                        SortDirection = request.SortDirection
                    });
                    _response = new APIResponse<IEnumerable<Employee>>() { Status = true, Msg = "Ok", Value = employees };

                    // cache employees
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(_cacheSettings.Value.SlidingExpirationMinutes))
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(_cacheSettings.Value.AbsoluteExpirationMinutes))
                        .SetSize(_cacheSettings.Value.Size);
                    _cacheManager.Set(cacheKey, employees, cacheEntryOptions);
                }

                return Ok(_response);
            }
            catch
            {
                _response = new APIResponse<IEnumerable<Employee>>() { Status = false, Msg = "An error occured!" };
                return StatusCode(500, _response);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User, Administrator")]
        public async Task<ActionResult> PostEmployee(Employee employee)
        {
            // get response
            APIResponse<Employee> _response = new APIResponse<Employee>();

            if (!ModelState.IsValid)
                return BadRequest(new APIResponse<Employee>() { Status = false, Msg = "Invalid model!"});

            try
            {
                // save employee
                _dataProvider.SaveEmployee(new wpsp_Employee_Save
                {
                    DepartmentId = employee.DepartmentId,
                    EmployeeName = employee.EmployeeName,
                    Salary = employee.Salary,
                    DateJoined = employee.DateJoined
                });
                _response = new APIResponse<Employee>() { Status = true, Msg = "Ok" };

                // clear cache
                _cacheManager.ClearAll();

                return Ok(_response);
            }
            catch
            {
                _response = new APIResponse<Employee>() { Status = false, Msg = "Could not add employee!" };
                return StatusCode(500, _response);
            }
        }

        [HttpPut]
        [Authorize(Roles = "User, Administrator")]
        public async Task<ActionResult> PutEmployee(Employee employee)
        {
            // get response
            APIResponse<Employee> _response = new APIResponse<Employee>();

            if (!ModelState.IsValid)
                return BadRequest(new APIResponse<Employee>() { Status = false, Msg = "Invalid model!" });

            try
            {
                // get employee
                Employee existingEmployee = await _dataProvider.GetEmployee(new wpsp_Employees_Select { EmployeeId = employee.EmployeeId });
                if (existingEmployee == null)
                {
                    // clear cache
                    _cacheManager.ClearAll();

                    return StatusCode(500, new APIResponse<Employee>() { Status = false, Msg = "Employee does not exist!" });
                }

                // save employee
                _dataProvider.SaveEmployee(new wpsp_Employee_Save
                {
                    EmployeeId = employee.EmployeeId,
                    DepartmentId = employee.DepartmentId,
                    EmployeeName = employee.EmployeeName,
                    Salary = employee.Salary,
                    DateJoined = employee.DateJoined
                });
                _response = new APIResponse<Employee>() { Status = true, Msg = "Ok" };

                // clear cache
                _cacheManager.ClearAll();

                return Ok(_response);
            }
            catch
            {
                _response = new APIResponse<Employee>() { Status = false, Msg = "Could not update employee!" };
                return StatusCode(500, _response);
            }
        }

        [HttpDelete("{employeeId}")]
        [Authorize(Roles = "User, Administrator")]
        public async Task<ActionResult> DeleteEmployee(int employeeId)
        {
            // get response
            APIResponse<bool> _response = new APIResponse<bool>();

            try
            {
                // get employee
                Employee existingEmployee = await _dataProvider.GetEmployee(new wpsp_Employees_Select { EmployeeId = employeeId });
                if (existingEmployee == null)
                {
                    // clear cache
                    _cacheManager.ClearAll();

                    return StatusCode(500, new APIResponse<Employee>() { Status = false, Msg = "Employee does not exist!" });
                }

                // save employee
                _dataProvider.DeleteEmployee(new wpsp_Employee_Delete
                {
                    EmployeeId = employeeId
                });
                _response = new APIResponse<bool>() { Status = true, Msg = "Ok", Value = true };

                // clear cache
                _cacheManager.ClearAll();

                return Ok(_response);
            }
            catch
            {
                _response = new APIResponse<bool>() { Status = false, Msg = "Could not delete employee!" };
                return StatusCode(500, _response);
            }
        }
    }
}
