using AbsenceManagementSystem.Core.Domain;
using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.IServices;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace AbsenceManagementSystem.Test
{
    public class EmployeeServiceTests
    {
        private readonly Mock<UserManager<Employee>> _userManagerMock;
        private readonly IEmployeeService _employeeService;

        public EmployeeServiceTests(IEmployeeService employeeService, Mock<UserManager<Employee>> userManagerMock)
        {
            _employeeService = employeeService;
            _userManagerMock = userManagerMock;
        }

        // Helper method to mock UserManager
        private Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            return new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
        }

        [Fact]
        public async Task AddNewEmployeeAsync_ShouldThrowArgumentNullException_WhenUserIsNull()
        {
            // Act
            Func<Task> act = async () => await _employeeService.AddNewEmployeeAsync(null);

            // Assert
            var sd = await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AddNewEmployeeAsync_ShouldReturnSucceededResponse_WhenUserIsCreatedSuccessfully()
        {
            // Arrange
            var user = new EmployeeDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "Password123!",
                PhoneNumber = "1234567890",
                UserName = "john.doe",
                Gender = "Male",
                MaritalStatus = "Single"
            };

            _userManagerMock
                .Setup(um => um.CreateAsync(It.IsAny<Employee>(), user.Password))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock
                .Setup(um => um.AddToRoleAsync(It.IsAny<Employee>(), "Employee"))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _employeeService.AddNewEmployeeAsync(user);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().Be(user);
            result.StatusCode.Should().Be(200);
            _userManagerMock.Verify(um => um.CreateAsync(It.IsAny<Employee>(), user.Password), Times.Once);
            _userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<Employee>(), "Employee"), Times.Once);
        }

        [Fact]
        public async Task AddNewEmployeeAsync_ShouldReturnFailedResponse_WhenUserCreationFails()
        {
            // Arrange
            var user = new EmployeeDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "Password123!",
                PhoneNumber = "1234567890",
                UserName = "john.doe",
                Gender = "Male",
                MaritalStatus = "Single"
            };

            var identityErrors = new IdentityError[]
            {
                new IdentityError { Description = "Password is too weak." }
            };

            _userManagerMock
                .Setup(um => um.CreateAsync(It.IsAny<Employee>(), user.Password))
                .ReturnsAsync(IdentityResult.Failed(identityErrors));

            // Act
            var result = await _employeeService.AddNewEmployeeAsync(user);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.StatusCode.Should().Be(500);
            result.Message.Should().Contain("Password is too weak.");
            _userManagerMock.Verify(um => um.CreateAsync(It.IsAny<Employee>(), user.Password), Times.Once);
            _userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<Employee>(), "Employee"), Times.Never);
        }

        [Fact]
        public async Task AddNewEmployeeAsync_ShouldHandleException()
        {
            // Arrange
            var user = new EmployeeDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "Password123!",
                PhoneNumber = "1234567890",
                UserName = "john.doe",
                Gender = "Male",
                MaritalStatus = "Single"
            };

            _userManagerMock
                .Setup(um => um.CreateAsync(It.IsAny<Employee>(), user.Password))
                .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _employeeService.AddNewEmployeeAsync(user);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.StatusCode.Should().Be(500);
            result.Message.Should().Contain("Something went wrong");
        }
    }
}
