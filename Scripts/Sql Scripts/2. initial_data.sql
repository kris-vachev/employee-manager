IF OBJECT_ID('Users', 'U') IS NOT NULL
BEGIN
    -- USERS table exists, perform the insert
    SET IDENTITY_INSERT [dbo].[Users] ON;
    INSERT INTO [dbo].[Users] (UserId, UserName, PasswordHash)
    VALUES (1, 'admin', 'AQAAAAEAACcQAAAAEI3s4N2wfvtjRwD593D2rD1TSf1RmMCskixOZIWYWCrwqwQHQZtgqIIug1XPcnyQOA=='),
           (2, 'user', 'AQAAAAEAACcQAAAAEP0DS1LIRG/CtHS7pti7yy5mdnMDjixMf2cUTjxerzstJhFS82BOz2EQwpL3gPsPBQ==');
    SET IDENTITY_INSERT [dbo].[Users] OFF;
END

IF OBJECT_ID('Roles', 'U') IS NOT NULL
BEGIN
    -- ROLES table exists, perform the insert
    SET IDENTITY_INSERT [dbo].[Roles] ON;
    INSERT INTO [dbo].[Roles] (RoleId, RoleName)
    VALUES (1, 'Administrator'),
           (2, 'User');
    SET IDENTITY_INSERT [dbo].[Roles] OFF;
END

IF OBJECT_ID('UserRoles', 'U') IS NOT NULL
BEGIN
    -- USER ROLES table exists, perform the insert
    INSERT INTO [dbo].[UserRoles] (UserId, RoleId)
    VALUES (1, 1),
           (2, 2);
END

IF OBJECT_ID('Departments', 'U') IS NOT NULL
BEGIN
    -- DEPARTMENTS table exists, perform the insert
    SET IDENTITY_INSERT [dbo].[Departments] ON;
    INSERT INTO [dbo].[Departments] (DepartmentId, DepartmentName)
    VALUES (1, 'IT'),
           (2, 'HR');
    SET IDENTITY_INSERT [dbo].[Departments] OFF;
END

IF OBJECT_ID('Employees', 'U') IS NOT NULL
BEGIN
    -- EMPLOYEES table exists, perform the insert
    SET IDENTITY_INSERT [dbo].[Employees] ON;
    INSERT INTO [dbo].[Employees] (EmployeeId, DepartmentId, EmployeeName, Salary, DateJoined)
    VALUES (1, 1, 'John Smith', 2000, '2021-05-04'),
           (2, 1, 'Jane Doe', 1500, '2022-02-06'),
           (3, 2, 'Bill Harvey', 2450, '2019-07-10');
    SET IDENTITY_INSERT [dbo].[Employees] OFF;
END