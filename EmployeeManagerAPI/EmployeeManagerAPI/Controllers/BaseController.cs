using EmployeeManagerAPI.Interfaces;
using EmployeeManagerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using System.Text;

namespace EmployeeManagerAPI.Controllers
{
    public class BaseController : ControllerBase
    {
        public BaseController() { }

        /// <summary>
        /// Extract the request parameters and append them to a custom cache key string.
        /// </summary>
        [NonAction]
        public static string GetCacheKey(string baseKey, APIRequest? request)
        {
            var cacheKey = new StringBuilder(baseKey);
            if (request != null)
            {
                if (request.PageNumber.HasValue && request.PageSize.HasValue)
                {
                    cacheKey.Append($"_{request.PageNumber}_{request.PageSize}");
                }
                if (request.FilterValue != null)
                {
                    cacheKey.Append($"_{request.FilterValue}");
                }
                if (request.SortField != null && request.SortDirection != null)
                {
                    cacheKey.Append($"_{request.SortField}_{request.SortDirection}");
                }
            }

            return cacheKey.ToString();
        }

    }
}
