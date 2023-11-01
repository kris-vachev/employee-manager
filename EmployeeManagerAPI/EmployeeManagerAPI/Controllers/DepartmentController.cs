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
using static EmployeeManagerAPI.Infrastructure.Models.Database;

namespace EmployeeManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : BaseController
    {
        private readonly string _baseCacheKey = "DepartmentList";
        private readonly IDepartmentDataProvider _dataProvider;
        private readonly IOptions<CacheSettings> _cacheSettings;
        private ICacheManager _cacheManager;

        public DepartmentController(IDepartmentDataProvider dataProvider, IOptions<CacheSettings> cacheSettings, ICacheManager cacheManager)
        {
            _dataProvider = dataProvider;
            _cacheSettings = cacheSettings;
            _cacheManager = cacheManager;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> GetDepartments([FromQuery]APIRequest request)
        {
            // get response
            APIResponse<IEnumerable<Department>> _response = new APIResponse<IEnumerable<Department>>();

            try
            {
                // get cached value
                // - get cache key
                string cacheKey = GetCacheKey(_baseCacheKey, request);
                // - get cached value
                IEnumerable<Department> departments = _cacheManager.Get<IEnumerable<Department>>(cacheKey);
                if (departments != null)
                {
                    _response = new APIResponse<IEnumerable<Department>>() { Status = true, Msg = "Ok", Value = departments };
                }
                else
                {
                    // get departments from DB
                    departments = await _dataProvider.GetDepartments(new wpsp_Departments_Select
                    {
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize,
                        Filter = request.FilterValue,
                        SortField = request.SortField,
                        SortDirection = request.SortDirection
                    });
                    _response = new APIResponse<IEnumerable<Department>>() { Status = true, Msg = "Ok", Value = departments };

                    // cache departments
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(_cacheSettings.Value.SlidingExpirationMinutes))
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(_cacheSettings.Value.AbsoluteExpirationMinutes))
                        .SetSize(_cacheSettings.Value.Size);
                    _cacheManager.Set(cacheKey, departments, cacheEntryOptions);
                }

                return Ok(_response);
            }
            catch
            {
                _response = new APIResponse<IEnumerable<Department>>() { Status = false, Msg = "An error occured!" };
                return StatusCode(500, _response);
            }
        }

        [HttpGet("list")]
        [Authorize(Roles = "User, Administrator")]
        public async Task<ActionResult> GetDepartmentList()
        {
            // get response
            APIResponse<IEnumerable<Department>> _response = new APIResponse<IEnumerable<Department>>();

            try
            {
                // get cached value
                // - get cache key
                string cacheKey = GetCacheKey(_baseCacheKey, null);
                // - get cached value
                IEnumerable<Department> departments = _cacheManager.Get<IEnumerable<Department>>(cacheKey);
                if (departments != null)
                {
                    _response = new APIResponse<IEnumerable<Department>>() { Status = true, Msg = "Ok", Value = departments };
                }
                else
                {
                    // get departments from DB
                    departments = await _dataProvider.GetDepartments(new wpsp_Departments_Select { });
                    _response = new APIResponse<IEnumerable<Department>>() { Status = true, Msg = "Ok", Value = departments };

                    // cache departments
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(_cacheSettings.Value.SlidingExpirationMinutes))
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(_cacheSettings.Value.AbsoluteExpirationMinutes))
                        .SetSize(_cacheSettings.Value.Size);
                    _cacheManager.Set(cacheKey, departments, cacheEntryOptions);
                }

                return Ok(_response);
            }
            catch
            {
                _response = new APIResponse<IEnumerable<Department>>() { Status = false, Msg = "An error occured!" };
                return StatusCode(500, _response);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> PostDepartment(Department department)
        {                
            // get response
            APIResponse<Department> _response = new APIResponse<Department>();

            if (!ModelState.IsValid)
                return BadRequest(new APIResponse<Department>() { Status = false, Msg = "Invalid model!" });

            try
            {
                // save department
                _dataProvider.SaveDepartment(new wpsp_Department_Save
                {
                    DepartmentName = department.DepartmentName
                });
                _response = new APIResponse<Department>() { Status = true, Msg = "Ok" };

                // clear cache
                _cacheManager.ClearAll();

                return Ok(_response);
            }
            catch
            {
                _response = new APIResponse<Department>() { Status = false, Msg = "Could not add department!" };
                return StatusCode(500, _response);
            }
        }

        [HttpPut]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> PutDepartment(Department department)
        {
            // get response
            APIResponse<Department> _response = new APIResponse<Department>();

            if (!ModelState.IsValid)
                return BadRequest(new APIResponse<Department>() { Status = false, Msg = "Invalid model!" });

            try
            {
                // get department
                Department existingDepartment = await _dataProvider.GetDepartment(new wpsp_Departments_Select { DepartmentId = department.DepartmentId });
                if (existingDepartment == null)
                {
                    // clear cache
                    _cacheManager.ClearAll();

                    return StatusCode(500, new APIResponse<Department>() { Status = false, Msg = "Department does not exist!" });
                }

                // save department
                _dataProvider.SaveDepartment(new wpsp_Department_Save
                {
                    DepartmentId = department.DepartmentId,
                    DepartmentName = department.DepartmentName
                });
                _response = new APIResponse<Department>() { Status = true, Msg = "Ok" };

                // clear cache
                _cacheManager.ClearAll();

                return Ok(_response);
            }
            catch
            {
                _response = new APIResponse<Department>() { Status = false, Msg = "Could not update department!" };
                return StatusCode(500, _response);
            }
        }

        [HttpDelete("{departmentId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> DeleteDepartment(int departmentId)
        {
            // get response
            APIResponse<bool> _response = new APIResponse<bool>();

            try
            {
                // get department
                Department existingDepartment = await _dataProvider.GetDepartment(new wpsp_Departments_Select { DepartmentId = departmentId });
                if (existingDepartment == null)
                {
                    // clear cache
                    _cacheManager.ClearAll();

                    return StatusCode(500, new APIResponse<Department>() { Status = false, Msg = "Department does not exist!" });
                }

                // delete department
                _dataProvider.DeleteDepartment(new wpsp_Department_Delete
                {
                    DepartmentId = departmentId
                });
                _response = new APIResponse<bool>() { Status = true, Msg = "Ok", Value = true };

                // clear cache
                _cacheManager.ClearAll();

                return Ok(_response);
            }
            catch
            {
                _response = new APIResponse<bool>() { Status = false, Msg = "Could not delete department!" };
                return StatusCode(500, _response);
            }
        }
    }
}
