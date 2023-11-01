using EmployeeManagerAPI.Controllers;
using EmployeeManagerAPI.Infrastructure.Interfaces;
using EmployeeManagerAPI.Infrastructure.Models;
using EmployeeManagerAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EmployeeManagerAPI.Infrastructure.Models.Database;

namespace EmployeeManagerAPI.UnitTests.Controllers
{
    public class EmployeeControllerUnitTests
    {
        [Fact]
        public async Task GetEmployees_ReturnsOkResultFromCache()
        {
            // Arrange
            var dataProviderMock = new Mock<IEmployeeDataProvider>();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var cacheSettingsOptions = Options.Create(configuration.GetSection("CacheSettings").Get<CacheSettings>());
            var cacheManagerMock = new Mock<ICacheManager>();
            var controller = new EmployeeController(dataProviderMock.Object, cacheSettingsOptions, cacheManagerMock.Object);
            var request = new APIRequest();

            // Mock the behavior of CacheManager
            var employees = new List<Employee>
    {
        new Employee { EmployeeId = 1, EmployeeName = "John Doe", Salary = 5000 },
        new Employee { EmployeeId = 2, EmployeeName = "Jane Smith", Salary = 6000 }
    };
            cacheManagerMock.Setup(c => c.Get<IEnumerable<Employee>>(It.IsAny<string>())).Returns(employees);

            // Act
            var result = await controller.GetEmployees(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            var response = result.Value as APIResponse<IEnumerable<Employee>>;
            Assert.NotNull(response);
            Assert.True(response.Status);
            Assert.NotNull(response.Msg);
            Assert.NotNull(response.Value);
            Assert.Equal(employees, response.Value);
            // Ensure DataProvider methods are not called
            dataProviderMock.Verify(d => d.GetEmployees(It.IsAny<wpsp_Employees_Select>()), Times.Never);
        }

        [Fact]
        public async Task GetEmployees_ReturnsOkResultFromDataProvider()
        {
            // Arrange
            var dataProviderMock = new Mock<IEmployeeDataProvider>();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var cacheSettingsOptions = Options.Create(configuration.GetSection("CacheSettings").Get<CacheSettings>());
            var cacheManagerMock = new Mock<ICacheManager>();
            var controller = new EmployeeController(dataProviderMock.Object, cacheSettingsOptions, cacheManagerMock.Object);
            var request = new APIRequest();

            // Mock the behavior of DataProvider
            var employees = new List<Employee>
    {
        new Employee { EmployeeId = 1, EmployeeName = "John Doe", Salary = 5000 },
        new Employee { EmployeeId = 2, EmployeeName = "Jane Smith", Salary = 6000 }
    };
            dataProviderMock.Setup(d => d.GetEmployees(It.IsAny<wpsp_Employees_Select>())).ReturnsAsync(employees);

            // Mock the behavior of CacheManager
            cacheManagerMock.Setup(c => c.Get<IEnumerable<Employee>>(It.IsAny<string>())).Returns((IEnumerable<Employee>)null);

            // Act
            var result = await controller.GetEmployees(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            var response = result.Value as APIResponse<IEnumerable<Employee>>;
            Assert.NotNull(response);
            Assert.True(response.Status);
            Assert.NotNull(response.Msg);
            Assert.NotNull(response.Value);
            Assert.Equal(employees, response.Value);
            // Ensure DataProvider methods are called
            dataProviderMock.Verify(d => d.GetEmployees(It.IsAny<wpsp_Employees_Select>()), Times.Once);
        }

        [Fact]
        public async Task PostEmployee_ReturnsOkResult()
        {
            // Arrange
            var dataProviderMock = new Mock<IEmployeeDataProvider>();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var cacheSettingsOptions = Options.Create(configuration.GetSection("CacheSettings").Get<CacheSettings>());
            var cacheManagerMock = new Mock<ICacheManager>();
            var controller = new EmployeeController(dataProviderMock.Object, cacheSettingsOptions, cacheManagerMock.Object);
            var employee = new Employee
            {
                EmployeeId = 1,
                EmployeeName = "John Doe",
                Salary = 5000,
                DateJoined = DateTime.Now
            };

            // Act
            var result = await controller.PostEmployee(employee) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            var response = result.Value as APIResponse<Employee>;
            Assert.NotNull(response);
            Assert.True(response.Status);
            Assert.NotNull(response.Msg);
            // Ensure DataProvider methods are called
            dataProviderMock.Verify(d => d.SaveEmployee(It.IsAny<wpsp_Employee_Save>()), Times.Once);
            cacheManagerMock.Verify(c => c.ClearAll(), Times.Once);
        }

        [Fact]
        public async Task PutEmployee_ReturnsOkResult()
        {
            // Arrange
            var dataProviderMock = new Mock<IEmployeeDataProvider>();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var cacheSettingsOptions = Options.Create(configuration.GetSection("CacheSettings").Get<CacheSettings>());
            var cacheManagerMock = new Mock<ICacheManager>();
            var controller = new EmployeeController(dataProviderMock.Object, cacheSettingsOptions, cacheManagerMock.Object);
            var employee = new Employee
            {
                EmployeeId = 1,
                EmployeeName = "John Doe",
                Salary = 5000,
                DateJoined = DateTime.Now
            };

            // Mock the behavior of DataProvider
            var existingEmployee = new Employee
            {
                EmployeeId = employee.EmployeeId,
                EmployeeName = "John Doe",
                Salary = 5000,
                DateJoined = DateTime.Now
            };
            dataProviderMock.Setup(d => d.GetEmployee(It.IsAny<wpsp_Employees_Select>())).ReturnsAsync(existingEmployee);

            // Act
            var result = await controller.PutEmployee(employee) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            var response = result.Value as APIResponse<Employee>;
            Assert.NotNull(response);
            Assert.True(response.Status);
            Assert.NotNull(response.Msg);
            // Ensure DataProvider methods are called
            dataProviderMock.Verify(d => d.GetEmployee(It.IsAny<wpsp_Employees_Select>()), Times.Once);
            dataProviderMock.Verify(d => d.SaveEmployee(It.IsAny<wpsp_Employee_Save>()), Times.Once);
            cacheManagerMock.Verify(c => c.ClearAll(), Times.Once);
        }

        [Fact]
        public async Task PutEmployee_EmployeeDoesNotExist_ReturnsStatusCode500()
        {
            // Arrange
            var dataProviderMock = new Mock<IEmployeeDataProvider>();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var cacheSettingsOptions = Options.Create(configuration.GetSection("CacheSettings").Get<CacheSettings>());
            var cacheManagerMock = new Mock<ICacheManager>();
            var controller = new EmployeeController(dataProviderMock.Object, cacheSettingsOptions, cacheManagerMock.Object);
            var employee = new Employee
            {
                EmployeeId = 1,
                EmployeeName = "John Doe",
                Salary = 5000,
                DateJoined = DateTime.Now
            };

            // Mock the behavior of DataProvider
            dataProviderMock.Setup(d => d.GetEmployee(It.IsAny<wpsp_Employees_Select>())).ReturnsAsync((Employee)null);

            // Act
            var result = await controller.PutEmployee(employee) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            // Ensure DataProvider methods are called
            dataProviderMock.Verify(d => d.GetEmployee(It.IsAny<wpsp_Employees_Select>()), Times.Once);
            dataProviderMock.Verify(d => d.SaveEmployee(It.IsAny<wpsp_Employee_Save>()), Times.Never);
            cacheManagerMock.Verify(c => c.ClearAll(), Times.Once);
        }

        [Fact]
        public async Task DeleteEmployee_ReturnsOkResult()
        {
            // Arrange
            var dataProviderMock = new Mock<IEmployeeDataProvider>();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var cacheSettingsOptions = Options.Create(configuration.GetSection("CacheSettings").Get<CacheSettings>());
            var cacheManagerMock = new Mock<ICacheManager>();
            var controller = new EmployeeController(dataProviderMock.Object, cacheSettingsOptions, cacheManagerMock.Object);
            var employeeId = 1;

            // Mock the behavior of DataProvider
            var existingEmployee = new Employee
            {
                EmployeeId = employeeId,
                EmployeeName = "John Doe",
                Salary = 5000,
                DateJoined = DateTime.Now
            };
            dataProviderMock.Setup(d => d.GetEmployee(It.IsAny<wpsp_Employees_Select>())).ReturnsAsync(existingEmployee);

            // Act
            var result = await controller.DeleteEmployee(employeeId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            var response = result.Value as APIResponse<bool>;
            Assert.NotNull(response);
            Assert.True(response.Status);
            Assert.NotNull(response.Msg);
            // Ensure DataProvider methods are called
            dataProviderMock.Verify(d => d.GetEmployee(It.IsAny<wpsp_Employees_Select>()), Times.Once);
            dataProviderMock.Verify(d => d.DeleteEmployee(It.IsAny<wpsp_Employee_Delete>()), Times.Once);
            cacheManagerMock.Verify(c => c.ClearAll(), Times.Once);
        }

        [Fact]
        public async Task DeleteEmployee_EmployeeDoesNotExist_ReturnsStatusCode500()
        {
            // Arrange
            var dataProviderMock = new Mock<IEmployeeDataProvider>();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var cacheSettingsOptions = Options.Create(configuration.GetSection("CacheSettings").Get<CacheSettings>());
            var cacheManagerMock = new Mock<ICacheManager>();
            var controller = new EmployeeController(dataProviderMock.Object, cacheSettingsOptions, cacheManagerMock.Object);
            var employeeId = 1;

            // Mock the behavior of DataProvider
            dataProviderMock.Setup(d => d.GetEmployee(It.IsAny<wpsp_Employees_Select>())).ReturnsAsync((Employee)null);

            // Act
            var result = await controller.DeleteEmployee(employeeId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            // Ensure DataProvider methods are called
            dataProviderMock.Verify(d => d.GetEmployee(It.IsAny<wpsp_Employees_Select>()), Times.Once);
            dataProviderMock.Verify(d => d.DeleteEmployee(It.IsAny<wpsp_Employee_Delete>()), Times.Never);
            cacheManagerMock.Verify(c => c.ClearAll(), Times.Once);
        }

    }
}
