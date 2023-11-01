SET ANSI_NULLS ON
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