SET ANSI_NULLS ON
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