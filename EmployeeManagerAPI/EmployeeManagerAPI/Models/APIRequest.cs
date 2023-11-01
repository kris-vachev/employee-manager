using EmployeeManagerAPI.Interfaces;

namespace EmployeeManagerAPI.Models
{
    public class APIRequest : IPagination, IFilter, ISort
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public int? TotalCount { get; set; }
        public string? FilterValue { get; set; }
        public string? SortField { get; set; }
        public string? SortDirection { get; set; }
    }
}
