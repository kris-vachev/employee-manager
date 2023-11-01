using EmployeeManagerAPI.Controllers;
using EmployeeManagerAPI.Infrastructure.Helpers;
using EmployeeManagerAPI.Infrastructure.Interfaces;
using EmployeeManagerAPI.Infrastructure.Models;
using EmployeeManagerAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
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
    public class DepartmentControllerUnitTests
    {
        [Fact]
        public async Task GetDepartments_ReturnsOkResultFromCache()
        {
            // Arrange
            var dataProviderMock = new Mock<IDepartmentDataProvider>();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var cacheSettingsOptions = Options.Create(configuration.GetSection("CacheSettings").Get<CacheSettings>());
            var cacheManagerMock = new Mock<ICacheManager>();
            var controller = new DepartmentController(dataProviderMock.Object, cacheSettingsOptions, cacheManagerMock.Object);
            var request = new APIRequest();

            // Mock the behavior of CacheManager
            var departments = new List<Department>
            {
                new Department { DepartmentId = 1, DepartmentName = "IT" },
                new Department { DepartmentId = 2, DepartmentName = "HR" }
            };
            cacheManagerMock.Setup(c => c.Get<IEnumerable<Department>>(It.IsAny<string>())).Returns(departments);

            // Act
            var result = await controller.GetDepartments(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            var response = result.Value as APIResponse<IEnumerable<Department>>;
            Assert.NotNull(response);
            Assert.True(response.Status);
            Assert.NotNull(response.Msg);
            Assert.NotNull(response.Value);
            Assert.Equal(departments, response.Value);
            // Ensure DataProvider methods are not called
            dataProviderMock.Verify(d => d.GetDepartments(It.IsAny<wpsp_Departments_Select>()), Times.Never);
        }

        [Fact]
        public async Task GetDepartments_ReturnsOkResultFromDataProvider()
        {
            // Arrange
            var dataProviderMock = new Mock<IDepartmentDataProvider>();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var cacheSettingsOptions = Options.Create(configuration.GetSection("CacheSettings").Get<CacheSettings>());
            var cacheManagerMock = new Mock<ICacheManager>();
            var controller = new DepartmentController(dataProviderMock.Object, cacheSettingsOptions, cacheManagerMock.Object);
            var request = new APIRequest();

            // Mock the behavior of DataProvider
            var departments = new List<Department>
            {
                new Department { DepartmentId = 1, DepartmentName = "IT" },
                new Department { DepartmentId = 2, DepartmentName = "HR" }
            };
            dataProviderMock.Setup(d => d.GetDepartments(It.IsAny<wpsp_Departments_Select>())).ReturnsAsync(departments);

            // Mock the behavior of CacheManager
            cacheManagerMock.Setup(c => c.Get<IEnumerable<Department>>(It.IsAny<string>())).Returns((IEnumerable<Department>)null);

            // Act
            var result = await controller.GetDepartments(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            var response = result.Value as APIResponse<IEnumerable<Department>>;
            Assert.NotNull(response);
            Assert.True(response.Status);
            Assert.NotNull(response.Msg);
            Assert.NotNull(response.Value);
            Assert.Equal(departments, response.Value);
            // Ensure DataProvider methods are called
            dataProviderMock.Verify(d => d.GetDepartments(It.IsAny<wpsp_Departments_Select>()), Times.Once);
        }

        [Fact]
        public async Task GetDepartmentList_ReturnsOkResultFromCache()
        {
            // Arrange
            var dataProviderMock = new Mock<IDepartmentDataProvider>();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var cacheSettingsOptions = Options.Create(configuration.GetSection("CacheSettings").Get<CacheSettings>());
            var cacheManagerMock = new Mock<ICacheManager>();
            var controller = new DepartmentController(dataProviderMock.Object, cacheSettingsOptions, cacheManagerMock.Object);

            // Mock the behavior of CacheManager
            var departments = new List<Department>
            {
                new Department { DepartmentId = 1, DepartmentName = "IT" },
                new Department { DepartmentId = 2, DepartmentName = "HR" }
            };
            cacheManagerMock.Setup(c => c.Get<IEnumerable<Department>>(It.IsAny<string>())).Returns(departments);

            // Act
            var result = await controller.GetDepartmentList() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            var response = result.Value as APIResponse<IEnumerable<Department>>;
            Assert.NotNull(response);
            Assert.True(response.Status);
            Assert.NotNull(response.Msg);
            Assert.NotNull(response.Value);
            Assert.Equal(departments, response.Value);
            // Ensure DataProvider methods are not called
            dataProviderMock.Verify(d => d.GetDepartments(It.IsAny<wpsp_Departments_Select>()), Times.Never);
        }

        [Fact]
        public async Task GetDepartmentList_ReturnsOkResultFromDataProvider()
        {
            // Arrange
            var dataProviderMock = new Mock<IDepartmentDataProvider>();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var cacheSettingsOptions = Options.Create(configuration.GetSection("CacheSettings").Get<CacheSettings>());
            var cacheManagerMock = new Mock<ICacheManager>();
            var controller = new DepartmentController(dataProviderMock.Object, cacheSettingsOptions, cacheManagerMock.Object);

            // Mock the behavior of DataProvider
            var departments = new List<Department>
            {
                new Department { DepartmentId = 1, DepartmentName = "IT" },
                new Department { DepartmentId = 2, DepartmentName = "HR" }
            };
            dataProviderMock.Setup(d => d.GetDepartments(It.IsAny<wpsp_Departments_Select>())).ReturnsAsync(departments);

            // Mock the behavior of CacheManager
            cacheManagerMock.Setup(c => c.Get<IEnumerable<Department>>(It.IsAny<string>())).Returns((IEnumerable<Department>)null);

            // Act
            var result = await controller.GetDepartmentList() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            var response = result.Value as APIResponse<IEnumerable<Department>>;
            Assert.NotNull(response);
            Assert.True(response.Status);
            Assert.NotNull(response.Msg);
            Assert.NotNull(response.Value);
            Assert.Equal(departments, response.Value);
            // Ensure DataProvider methods are called
            dataProviderMock.Verify(d => d.GetDepartments(It.IsAny<wpsp_Departments_Select>()), Times.Once);
        }

        [Fact]
        public async Task PostDepartment_ReturnsOkResult()
        {
            // Arrange
            var dataProviderMock = new Mock<IDepartmentDataProvider>();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var cacheSettingsOptions = Options.Create(configuration.GetSection("CacheSettings").Get<CacheSettings>());
            var cacheManagerMock = new Mock<ICacheManager>();
            var controller = new DepartmentController(dataProviderMock.Object, cacheSettingsOptions, cacheManagerMock.Object);
            var department = new Department { DepartmentId = 1, DepartmentName = "IT" };

            // Act
            var result = await controller.PostDepartment(department) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            var response = result.Value as APIResponse<Department>;
            Assert.NotNull(response);
            Assert.True(response.Status);
            Assert.NotNull(response.Msg);
            // Ensure DataProvider methods are called
            dataProviderMock.Verify(d => d.SaveDepartment(It.IsAny<wpsp_Department_Save>()), Times.Once);
            cacheManagerMock.Verify(c => c.ClearAll(), Times.Once);
        }

        [Fact]
        public async Task PutDepartment_ReturnsOkResult()
        {
            // Arrange
            var dataProviderMock = new Mock<IDepartmentDataProvider>();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var cacheSettingsOptions = Options.Create(configuration.GetSection("CacheSettings").Get<CacheSettings>());
            var cacheManagerMock = new Mock<ICacheManager>();
            var controller = new DepartmentController(dataProviderMock.Object, cacheSettingsOptions, cacheManagerMock.Object);
            var department = new Department { DepartmentId = 1, DepartmentName = "IT" };

            // Mock the behavior of DataProvider
            var existingDepartment = new Department { DepartmentId = department.DepartmentId, DepartmentName = "IT" };
            dataProviderMock.Setup(d => d.GetDepartment(It.IsAny<wpsp_Departments_Select>())).ReturnsAsync(existingDepartment);

            // Act
            var result = await controller.PutDepartment(department) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            var response = result.Value as APIResponse<Department>;
            Assert.NotNull(response);
            Assert.True(response.Status);
            Assert.NotNull(response.Msg);
            // Ensure DataProvider methods are called
            dataProviderMock.Verify(d => d.GetDepartment(It.IsAny<wpsp_Departments_Select>()), Times.Once);
            dataProviderMock.Verify(d => d.SaveDepartment(It.IsAny<wpsp_Department_Save>()), Times.Once);
            cacheManagerMock.Verify(c => c.ClearAll(), Times.Once);
        }

        [Fact]
        public async Task PutDepartment_DepartmentDoesNotExist_ReturnsStatusCode500()
        {
            // Arrange
            var dataProviderMock = new Mock<IDepartmentDataProvider>();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var cacheSettingsOptions = Options.Create(configuration.GetSection("CacheSettings").Get<CacheSettings>());
            var cacheManagerMock = new Mock<ICacheManager>();
            var controller = new DepartmentController(dataProviderMock.Object, cacheSettingsOptions, cacheManagerMock.Object);
            var department = new Department { DepartmentId = 1, DepartmentName = "IT" };

            // Mock the behavior of DataProvider
            dataProviderMock.Setup(d => d.GetDepartment(It.IsAny<wpsp_Departments_Select>())).ReturnsAsync((Department)null);

            // Act
            var result = await controller.PutDepartment(department) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            // Ensure DataProvider methods are called
            dataProviderMock.Verify(d => d.GetDepartment(It.IsAny<wpsp_Departments_Select>()), Times.Once);
            dataProviderMock.Verify(d => d.SaveDepartment(It.IsAny<wpsp_Department_Save>()), Times.Never);
            cacheManagerMock.Verify(c => c.ClearAll(), Times.Once);
            cacheManagerMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<IEnumerable<Department>>(), It.IsAny<MemoryCacheEntryOptions>()), Times.Never);
        }

        [Fact]
        public async Task DeleteDepartment_ReturnsOkResult()
        {
            // Arrange
            var dataProviderMock = new Mock<IDepartmentDataProvider>();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var cacheSettingsOptions = Options.Create(configuration.GetSection("CacheSettings").Get<CacheSettings>());
            var cacheManagerMock = new Mock<ICacheManager>();
            var controller = new DepartmentController(dataProviderMock.Object, cacheSettingsOptions, cacheManagerMock.Object);
            var departmentId = 1;

            // Mock the behavior of DataProvider
            var existingDepartment = new Department { DepartmentId = departmentId, DepartmentName = "IT" };
            dataProviderMock.Setup(d => d.GetDepartment(It.IsAny<wpsp_Departments_Select>())).ReturnsAsync(existingDepartment);

            // Act
            var result = await controller.DeleteDepartment(departmentId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            var response = result.Value as APIResponse<bool>;
            Assert.NotNull(response);
            Assert.True(response.Status);
            Assert.NotNull(response.Msg);
            // Ensure DataProvider methods are called
            dataProviderMock.Verify(d => d.GetDepartment(It.IsAny<wpsp_Departments_Select>()), Times.Once);
            dataProviderMock.Verify(d => d.DeleteDepartment(It.IsAny<wpsp_Department_Delete>()), Times.Once);
            cacheManagerMock.Verify(c => c.ClearAll(), Times.Once);
        }

        [Fact]
        public async Task DeleteDepartment_DepartmentDoesNotExist_ReturnsStatusCode500()
        {
            // Arrange
            var dataProviderMock = new Mock<IDepartmentDataProvider>();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var cacheSettingsOptions = Options.Create(configuration.GetSection("CacheSettings").Get<CacheSettings>());
            var cacheManagerMock = new Mock<ICacheManager>();
            var controller = new DepartmentController(dataProviderMock.Object, cacheSettingsOptions, cacheManagerMock.Object);
            var departmentId = 1;

            // Mock the behavior of DataProvider
            dataProviderMock.Setup(d => d.GetDepartment(It.IsAny<wpsp_Departments_Select>())).ReturnsAsync((Department)null);

            // Mock the behavior of CacheManager
            cacheManagerMock.Setup(c => c.ClearAll()).Callback(() => { /* Do nothing */ });

            // Act
            var result = await controller.DeleteDepartment(departmentId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            // Ensure DataProvider methods are called
            dataProviderMock.Verify(d => d.GetDepartment(It.IsAny<wpsp_Departments_Select>()), Times.Once);
            dataProviderMock.Verify(d => d.DeleteDepartment(It.IsAny<wpsp_Department_Delete>()), Times.Never);
            cacheManagerMock.Verify(c => c.ClearAll(), Times.Once);
        }
    }
}