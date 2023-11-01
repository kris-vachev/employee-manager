SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[wpsp_Employees_Select]') AND type in (N'P', N'PC'))
    EXEC ('CREATE PROCEDURE [dbo].[wpsp_Employees_Select] AS')
GO
ALTER PROCEDURE [dbo].[wpsp_Employees_Select]
(
    @EmployeeId int = NULL,
    @DepartmentId int = NULL,
    @EmployeeName varchar(255) = NULL,
    ----------------
    @PageNumber int = NULL,
    @PageSize int = NULL,
    ----------------
    @Filter varchar(max) = NULL,
    ----------------
    @SortField varchar(255) = NULL,
    @SortDirection varchar(255) = NULL
)
AS
BEGIN
    SET NOCOUNT ON
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    DECLARE @StartRow int, @EndRow int, @TotalCount int

    -- retrieve the total count of records without applying pagination
    SET @TotalCount = (SELECT COUNT(*) FROM Employees 
    LEFT JOIN Departments AS departments ON departments.DepartmentId = employees.DepartmentId 
    WHERE 
        (@EmployeeId IS NULL OR EmployeeId = @EmployeeId)
        AND (@DepartmentId IS NULL OR employees.DepartmentId = @DepartmentId)
        AND (@EmployeeName IS NULL OR EmployeeName = @EmployeeName)
        AND (@Filter IS NULL OR 
            (
            EmployeeName LIKE '%' + @Filter + '%' OR
            departments.DepartmentName LIKE '%' + @Filter + '%' OR
            Salary LIKE '%' + @Filter + '%' OR
            DateJoined LIKE '%' + @Filter + '%'
            )))

    -- calculate the start and end row numbers based on the page number and page size
    IF (@PageNumber IS NOT NULL AND @PageSize IS NOT NULL)
    BEGIN
        SET @StartRow = (@PageNumber - 1) * @PageSize + 1
        SET @EndRow = @StartRow + @PageSize - 1
    END

    -- apply pagination and retrieve the employees with department information
    ;WITH Pagination AS (
        SELECT ROW_NUMBER() OVER (ORDER BY
            CASE WHEN @SortField = 'EmployeeId' AND @SortDirection = 'ASC' THEN employees.EmployeeId END ASC,
            CASE WHEN @SortField = 'EmployeeId' AND @SortDirection = 'DESC' THEN employees.EmployeeId END DESC,
            CASE WHEN @SortField = 'DepartmentName' AND @SortDirection = 'ASC' THEN departments.DepartmentName END ASC,
            CASE WHEN @SortField = 'DepartmentName' AND @SortDirection= 'DESC' THEN departments.DepartmentName END DESC,
            CASE WHEN @SortField = 'EmployeeName' AND @SortDirection = 'ASC' THEN employees.EmployeeName END ASC,
            CASE WHEN @SortField = 'EmployeeName' AND @SortDirection= 'DESC' THEN employees.EmployeeName END DESC,
            CASE WHEN @SortField = 'Salary' AND @SortDirection = 'ASC' THEN employees.Salary END ASC,
            CASE WHEN @SortField = 'Salary' AND @SortDirection = 'DESC' THEN employees.Salary END DESC,
            CASE WHEN @SortField = 'DateJoined' AND @SortDirection = 'ASC' THEN employees.DateJoined END ASC,
            CASE WHEN @SortField = 'DateJoined' AND @SortDirection = 'DESC' THEN employees.DateJoined END DESC) AS RowNumber,
            employees.EmployeeId, employees.EmployeeName, employees.Salary, employees.DateJoined, 
            departments.DepartmentId, departments.DepartmentName
        FROM Employees AS employees
        LEFT JOIN Departments AS departments ON departments.DepartmentId = employees.DepartmentId
        WHERE (@EmployeeId IS NULL OR EmployeeId = @EmployeeId)
            AND (@DepartmentId IS NULL OR departments.DepartmentId = @DepartmentId)
            AND (@EmployeeName IS NULL OR EmployeeName = @EmployeeName)
            AND (@Filter IS NULL OR 
                (
                EmployeeName LIKE '%' + @Filter + '%' OR
                departments.DepartmentName LIKE '%' + @Filter + '%' OR
                Salary LIKE '%' + @Filter + '%' OR
                DateJoined LIKE '%' + @Filter + '%'
                ))
    )
    SELECT *,
        @PageNumber AS PageNumber,
        @PageSize AS PageSize,
        @TotalCount AS TotalCount
    FROM Pagination
    WHERE (@StartRow IS NULL OR RowNumber >= @StartRow)
        AND (@EndRow IS NULL OR RowNumber <= @EndRow)

    SET NOCOUNT OFF
END
GO