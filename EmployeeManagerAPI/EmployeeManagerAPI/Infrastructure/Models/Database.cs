namespace EmployeeManagerAPI.Infrastructure.Models
{
    public class Database
    {
        // Department ================================

        public class wpsp_Departments_Select
        {
            public int? DepartmentId { get; set; }
            public string? DepartmentName { get; set; }
            public int? PageNumber { get; set; }
            public int? PageSize { get; set; }
            public string? Filter { get; set; }
            public string? SortField { get; set; }
            public string? SortDirection { get; set; }
        }
        public class wpsp_Department_Save
        {
            public int? DepartmentId { get; set; }
            public string DepartmentName { get; set; }
        }
        public class wpsp_Department_Delete
        {
            public int DepartmentId { get; set; }
        }

        // Employee =================================

        public class wpsp_Employees_Select
        {
            public int? EmployeeId { get; set; }
            public int? DepartmentId { get; set; }
            public string EmployeeName { get; set; }
            public int? PageNumber { get; set; }
            public int? PageSize { get; set; }
            public string? Filter { get; set; }
            public string? SortField { get; set; }
            public string? SortDirection { get; set; }
        }
        public class wpsp_Employee_Save
        {
            public int? EmployeeId { get; set; }
            public int DepartmentId { get; set; }
            public string EmployeeName { get; set; }
            public int Salary { get; set; }
            public DateTime DateJoined { get; set; }
        }
        public class wpsp_Employee_Delete
        {
            public int EmployeeId { get; set; }
        }

        // User =====================================
        public class wpsp_User_Select
        {
            public int? UserId { get; set; }
            public string? UserName { get; set; }
        }
    }
}
