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