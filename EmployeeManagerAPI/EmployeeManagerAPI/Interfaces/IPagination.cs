namespace EmployeeManagerAPI.Interfaces
{
    public interface IPagination
    {
        int? PageNumber { get; set; }
        int? PageSize { get; set; }
        int? TotalCount { get; set; }
    }
}
