namespace EmployeeManagerAPI.Models
{
    public class APIResponse<T>
    {
        public bool Status { get; set; }
        public string? Msg { get; set; }
        public T? Value { get; set; }
    }
}
