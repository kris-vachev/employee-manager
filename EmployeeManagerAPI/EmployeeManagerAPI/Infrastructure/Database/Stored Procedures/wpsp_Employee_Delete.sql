SET ANSI_NULLS ON
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