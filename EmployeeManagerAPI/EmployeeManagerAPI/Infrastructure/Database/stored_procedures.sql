 
-- Stored Procedure: wpsp_Departments_Select 
﻿SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[wpsp_Departments_Select]') AND type in (N'P', N'PC'))
    EXEC ('CREATE PROCEDURE [dbo].[wpsp_Departments_Select] AS')
GO
ALTER PROCEDURE [dbo].[wpsp_Departments_Select]
(
    @DepartmentId int = NULL,
    @DepartmentName varchar(255) = NULL,
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

    -- calculate the start and end row numbers based on the page number and page size
    IF (@PageNumber IS NOT NULL AND @PageSize IS NOT NULL)
    BEGIN
        SET @StartRow = (@PageNumber - 1) * @PageSize + 1
        SET @EndRow = @StartRow + @PageSize - 1
    END

    -- retrieve the total count of records without applying pagination
    SET @TotalCount = (SELECT COUNT(*) FROM Departments WHERE (@DepartmentId IS NULL OR DepartmentId = @DepartmentId)
                                                      AND (@DepartmentName IS NULL OR DepartmentName = @DepartmentName)
                                                      AND (@Filter IS NULL OR DepartmentName LIKE '%' + @Filter + '%'))

    -- apply pagination and retrieve the departments with page number, page size, and total count
    ;WITH Pagination AS (
        SELECT ROW_NUMBER() OVER (ORDER BY
            CASE WHEN @SortField = 'DepartmentId' AND @SortDirection = 'ASC' THEN DepartmentId END ASC,
            CASE WHEN @SortField = 'DepartmentId' AND @SortDirection = 'DESC' THEN DepartmentId END DESC,
            CASE WHEN @SortField = 'DepartmentName' AND @SortDirection = 'ASC' THEN DepartmentName END ASC,
            CASE WHEN @SortField = 'DepartmentName' AND @SortDirection = 'DESC' THEN DepartmentName END DESC) AS RowNumber,
            *
        FROM Departments
        WHERE (@DepartmentId IS NULL OR DepartmentId = @DepartmentId)
              AND (@DepartmentName IS NULL OR DepartmentName = @DepartmentName)
              AND (@Filter IS NULL OR DepartmentName LIKE '%' + @Filter + '%')
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
 
-- Stored Procedure: wpsp_Department_Delete 
﻿SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[wpsp_Department_Delete]') AND type in (N'P', N'PC'))
	EXEC ('CREATE PROCEDURE [dbo].[wpsp_Department_Delete] AS')
GO
ALTER PROCEDURE[dbo].[wpsp_Department_Delete]
(
	@DepartmentId int
)
AS
BEGIN
SET NOCOUNT ON;

	DECLARE @IsInNestedTransaction BIT;

	BEGIN TRY

		IF (@@TRANCOUNT = 0)
			SET @IsInNestedTransaction = 0;
		ELSE
			SET @IsInNestedTransaction = 1;

		IF (@IsInNestedTransaction = 0)
			BEGIN TRAN; -- only start a transaction if not already in one
		-- ---------------------------------------------------------------

		-- DELETE EMPLOYEES
		DELETE FROM
			Employees
		WHERE
			DepartmentId = @DepartmentId

		-- DELETE DEPARTMENT
		DELETE FROM 
			Departments
		WHERE
			DepartmentId = @DepartmentId

		-- ---------------------------------------------------------------
			IF (@@TRANCOUNT > 0 AND @IsInNestedTransaction = 0)
				COMMIT TRAN;

	END TRY
	BEGIN CATCH

		IF (@@TRANCOUNT > 0 AND @IsInNestedTransaction = 0)
		BEGIN
			ROLLBACK;
		END;

		DECLARE @ErrorMessage   NVARCHAR(4000) = ERROR_MESSAGE(),
				@ErrorState     INT = ERROR_STATE(),
				@ErrorSeverity  INT = ERROR_SEVERITY();

		-- optionally concatenate ERROR_NUMBER() and/or ERROR_LINE() into @ErrorMessage

		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
		RETURN;

	END CATCH

END
GO 
 
-- Stored Procedure: wpsp_Department_Save 
﻿SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[wpsp_Department_Save]') AND type in (N'P', N'PC'))
	EXEC ('CREATE PROCEDURE [dbo].[wpsp_Department_Save] AS')
GO
ALTER PROCEDURE[dbo].[wpsp_Department_Save]
(
	@DepartmentId int = NULL,
	@DepartmentName varchar(255)
)
AS
BEGIN
	
	-- INSERT or UPDATE
	IF (@DepartmentId IS NULL)
		BEGIN
			INSERT INTO Departments (DepartmentName)
			VALUES (@DepartmentName)
			SET @DepartmentId = @@IDENTITY
		END
	ELSE IF EXISTS (SELECT 1 FROM Departments WHERE DepartmentId = @DepartmentId)
		BEGIN
			UPDATE Departments
			SET
				DepartmentName = @DepartmentName
			WHERE
				DepartmentId = @DepartmentId
		END
	ELSE
		BEGIN
			declare @msg nvarchar(256) = 'A department with the given ID (' + rtrim(cast(@DepartmentId as varchar(10))) + ') does not exist.'
			RAISERROR(@msg, 16, 1);
		END

END
GO 
 
-- Stored Procedure: wpsp_Employees_Select 
﻿SET ANSI_NULLS ON
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
 
-- Stored Procedure: wpsp_Employee_Delete 
﻿SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[wpsp_Employee_Delete]') AND type in (N'P', N'PC'))
	EXEC ('CREATE PROCEDURE [dbo].[wpsp_Employee_Delete] AS')
GO
ALTER PROCEDURE[dbo].[wpsp_Employee_Delete]
(
	@EmployeeId int
)
AS
BEGIN

	-- DELETE
	DELETE FROM 
		Employees
	WHERE
		EmployeeId = @EmployeeId

END
GO 
 
-- Stored Procedure: wpsp_Employee_Save 
﻿SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[wpsp_Employee_Save]') AND type in (N'P', N'PC'))
	EXEC ('CREATE PROCEDURE [dbo].[wpsp_Employee_Save] AS')
GO
ALTER PROCEDURE[dbo].[wpsp_Employee_Save]
(
	@EmployeeId int = NULL,
	@DepartmentId int,
	@EmployeeName varchar(255),
	@Salary int,
	@DateJoined date
)
AS
BEGIN
	
	-- INSERT or UPDATE
	IF (@EmployeeId IS NULL)
		BEGIN
			INSERT INTO Employees (DepartmentId, EmployeeName, Salary, DateJoined)
			VALUES (@DepartmentId, @EmployeeName, @Salary, @DateJoined)
			SET @EmployeeId = @@IDENTITY
		END
	ELSE IF EXISTS (SELECT 1 FROM Employees WHERE EmployeeId = @EmployeeId)
		BEGIN
			UPDATE Employees
			SET
				DepartmentId = @DepartmentId,
				EmployeeName = @EmployeeName,
				Salary = @Salary,
				DateJoined = @DateJoined
			WHERE
				EmployeeId = @EmployeeId
		END
	ELSE
		BEGIN
			declare @msg nvarchar(256) = 'An employee with the given ID (' + rtrim(cast(@EmployeeId as varchar(10))) + ') does not exist.'
			RAISERROR(@msg, 16, 1);
		END

END
GO 
 
-- Stored Procedure: wpsp_User_Select 
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[wpsp_User_Select]') AND type in (N'P', N'PC'))
	EXEC ('CREATE PROCEDURE [dbo].[wpsp_User_Select] AS')
GO
ALTER PROCEDURE[dbo].[wpsp_User_Select]
(
	@UserId int = NULL,
	@UserName varchar(255) = NULL
)
AS
-- exec wpsp_User_Select
BEGIN
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	SELECT
		[user].*,
		[role].*
	FROM
		Users AS [user]
	LEFT JOIN
		UserRoles AS userRoles ON userRoles.UserId = [user].UserId
	LEFT JOIN
		Roles AS [role] ON [role].RoleId = userRoles.RoleId
	WHERE
		(@UserId IS NULL OR [user].UserId = @UserId)
		AND
		(@UserName IS NULL OR [user].UserName = @UserName)

SET NOCOUNT OFF
END
GO 
