namespace EmployeeManagerAPI.Infrastructure.Models
{
    public class CacheSettings
    {
        public int AbsoluteExpirationMinutes { get; set; }
        public int SlidingExpirationMinutes { get; set; }
        public int Size { get; set; }
    }
}
