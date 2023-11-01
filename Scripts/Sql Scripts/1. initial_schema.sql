-- DATABASE
IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'EmployeeManagerDB')
BEGIN
	CREATE DATABASE EmployeeManagerDB;
END
GO

USE EmployeeManagerDB;
GO

-- TABLES ==============================================================

-- USERS
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Users](
		[UserId] [int] IDENTITY(1,1) NOT NULL,
		[UserName] [varchar](50) NOT NULL,
		[PasswordHash] [nvarchar](max) NOT NULL,
	 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
	(
		[UserId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

-- ROLES
IF OBJECT_ID(N'dbo.Roles', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Roles](
		[RoleId] [int] IDENTITY(1,1) NOT NULL,
		[RoleName] [varchar](50) NOT NULL,
	 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
	(
		[RoleId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

-- USER ROLES
IF OBJECT_ID(N'dbo.UserRoles', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[UserRoles](
		[UserId] [int] NOT NULL,
		[RoleId] [int] NOT NULL
	) ON [PRIMARY]

	ALTER TABLE [dbo].[UserRoles]  WITH CHECK ADD  CONSTRAINT [FK_UserRoles_Roles] FOREIGN KEY([RoleId])
	REFERENCES [dbo].[Roles] ([RoleId])

	ALTER TABLE [dbo].[UserRoles] CHECK CONSTRAINT [FK_UserRoles_Roles]

	ALTER TABLE [dbo].[UserRoles]  WITH CHECK ADD  CONSTRAINT [FK_UserRoles_Users] FOREIGN KEY([UserId])
	REFERENCES [dbo].[Users] ([UserId])

	ALTER TABLE [dbo].[UserRoles] CHECK CONSTRAINT [FK_UserRoles_Users]
END
GO

-- DEPARTMENTS
IF OBJECT_ID(N'dbo.Departments', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Departments](
		[DepartmentId] [int] IDENTITY(1,1) NOT NULL,
		[DepartmentName] [varchar](255) NOT NULL,
	 CONSTRAINT [PK_Departments] PRIMARY KEY CLUSTERED 
	(
		[DepartmentId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

-- EMPLOYEES
IF OBJECT_ID(N'dbo.Employees', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Employees](
		[EmployeeId] [int] IDENTITY(1,1) NOT NULL,
		[DepartmentId] [int] NOT NULL,
		[EmployeeName] [varchar](50) NOT NULL,
		[Salary] [int] NOT NULL,
		[DateJoined] [date] NOT NULL,
	 CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED 
	(
		[EmployeeId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[Employees]  WITH NOCHECK ADD  CONSTRAINT [FK_Employees_Departments] FOREIGN KEY([DepartmentId])
	REFERENCES [dbo].[Departments] ([DepartmentId])
	NOT FOR REPLICATION

	ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [FK_Employees_Departments]
END
GO