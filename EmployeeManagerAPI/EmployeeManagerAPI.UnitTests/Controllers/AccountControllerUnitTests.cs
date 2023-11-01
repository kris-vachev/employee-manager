using EmployeeManagerAPI.Controllers;
using EmployeeManagerAPI.Helpers;
using EmployeeManagerAPI.Infrastructure.Interfaces;
using EmployeeManagerAPI.Infrastructure.Models;
using EmployeeManagerAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static EmployeeManagerAPI.Infrastructure.Models.Database;

namespace EmployeeManagerAPI.UnitTests.Controllers
{
    public class AccountControllerUnitTests
    {
        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var dataProviderMock = new Mock<IUserDataProvider>();
            var configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .Build();
            var jwtSettingsOptions = Options.Create(configuration.GetSection("JwtSettings").Get<JwtSettings>());
            var controller = new AccountController(dataProviderMock.Object, jwtSettingsOptions);
            var login = new Login
            {
                UserName = "admin",
                Password = "1357$"
            };

            // Mock the behavior of DataProvider
            dataProviderMock.Setup(d => d.GetUser(It.IsAny<wpsp_User_Select>())).ReturnsAsync(new User
            {
                UserId = 1,
                UserName = "admin",
                PasswordHash = "AQAAAAEAACcQAAAAEI3s4N2wfvtjRwD593D2rD1TSf1RmMCskixOZIWYWCrwqwQHQZtgqIIug1XPcnyQOA==",
                Role = new Role { RoleId = 1, RoleName = "Admin" }
            });

            // Act
            var result = await controller.Login(login) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var response = result.Value as APIResponse<User>;
            Assert.NotNull(response);
            Assert.True(response.Status);
            Assert.NotNull(response.Msg);
            Assert.NotNull(response.Value);
            Assert.Equal("admin", response.Value.UserName);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsOkResultWithErrorMessage()
        {
            // Arrange
            var dataProviderMock = new Mock<IUserDataProvider>();
            var jwtSettingsMock = new Mock<IOptions<JwtSettings>>();
            var controller = new AccountController(dataProviderMock.Object, jwtSettingsMock.Object);
            var login = new Login
            {
                UserName = "admin",
                Password = "1234"
            };

            // Mock the behavior of DataProvider
            dataProviderMock.Setup(d => d.GetUser(It.IsAny<wpsp_User_Select>())).ReturnsAsync(new User
            {
                UserId = 1,
                UserName = "admin",
                PasswordHash = "AQAAAAEAACcQAAAAEI3s4N2wfvtjRwD593D2rD1TSf1RmMCskixOZIWYWCrwqwQHQZtgqIIug1XPcnyQOA==",
                Role = new Role { RoleId = 1, RoleName = "Admin" }
            });

            // Act
            var result = await controller.Login(login) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var response = result.Value as APIResponse<User>;
            Assert.NotNull(response);
            Assert.False(response.Status);
            Assert.Equal("Invalid credentials!", response.Msg);
            Assert.Null(response.Value);
        }

        [Fact]
        public async Task Login_UserNotFound_ReturnsOkResultWithErrorMessage()
        {
            // Arrange
            var dataProviderMock = new Mock<IUserDataProvider>();
            var jwtSettingsMock = new Mock<IOptions<JwtSettings>>();
            var controller = new AccountController(dataProviderMock.Object, jwtSettingsMock.Object);
            var login = new Login
            {
                UserName = "nonexistent",
                Password = "password"
            };

            // Mock the behavior of DataProvider
            dataProviderMock.Setup(d => d.GetUser(It.IsAny<wpsp_User_Select>())).ReturnsAsync((User)null);

            // Act
            var result = await controller.Login(login) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var response = result.Value as APIResponse<User>;
            Assert.NotNull(response);
            Assert.False(response.Status);
            Assert.Equal("User does not exist!", response.Msg);
            Assert.Null(response.Value);
        }
    }
}