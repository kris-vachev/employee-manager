namespace EmployeeManagerAPI.Interfaces
{
    public interface ISort
    {
        string? SortField { get; set; }
        string? SortDirection { get; set; }
    }
}