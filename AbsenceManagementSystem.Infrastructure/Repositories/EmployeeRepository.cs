using AbsenceManagementSystem.Core.Domain;
using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.Enums;
using AbsenceManagementSystem.Core.IRepositories;
using AbsenceManagementSystem.Infrastructure.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AbsenceManagementSystem.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AMSDbContext _dbContext;
        private readonly UserManager<Employee> _userManager;
        public EmployeeRepository(AMSDbContext dbContext, UserManager<Employee> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<EmployeeDto> AddNewEmployeeAsync(EmployeeDto user)
        {
            if (user == null)
            {
                throw new ArgumentNullException();
            }

            Employee employee = new Employee()
            {
                Id = Guid.NewGuid().ToString(),
                StartDate = DateTime.Now,
                MaritalStatus = user.MaritalStatus,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
                DateOfBirth = DateTime.Now,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Gender = user.Gender,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                PhoneNumber = user.PhoneNumber,
                IsActive = true,
                TotalHolidayEntitlement = 100,
                ContractType = ContractType.FullTime
            };


            IdentityResult result = await _userManager.CreateAsync(employee, user.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(employee, "Employee");
                return user;

            }

            string errors = string.Empty;
            foreach (var error in result.Errors)
            {
                errors += error.Description + Environment.NewLine;
            }

            throw new MissingFieldException(errors);
        }

        public async Task<List<EmployeeDto>> GetAllEmployeesAsync()
        {
            var employees = await _userManager.Users.Select(x => new EmployeeDto
            {
                StartDate = DateTime.Now,
                MaritalStatus = x.MaritalStatus,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
                DateOfBirth = DateTime.Now,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                UserName = x.UserName,
                Gender = x.Gender,
                PhoneNumber = x.PhoneNumber,
                TotalHolidayEntitlement = 100,
                ContractType = ContractType.FullTime
            }).ToListAsync();

            if (employees != null)
            {
                return employees;
            }
            
            return new List<EmployeeDto>();
        }
    }
}
