SET ANSI_NULLS ON
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