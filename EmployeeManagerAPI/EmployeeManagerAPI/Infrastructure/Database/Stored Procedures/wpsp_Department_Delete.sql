SET ANSI_NULLS ON
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