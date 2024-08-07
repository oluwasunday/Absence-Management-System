﻿using AbsenceManagementSystem.Core.Domain;
using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.Enums;
using AbsenceManagementSystem.Core.Handlers;
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
                EmployeeId = x.Id,
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

        public async Task<Response<EmployeeDto>> GetEmployeeByIdAsync(string employeeId)
        {
            var response = new Response<EmployeeDto>()
            {
                StatusCode = 200,
                Data = new EmployeeDto(),
                Succeeded = false,
                Errors = null,
                Message = string.Empty
            };

            try
            {
                var employee = await _userManager.Users.Where(x => x.Id == employeeId && x.IsActive)
                    .Select(x => new EmployeeDto
                    {
                        EmployeeId = x.Id,
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
                    }).FirstOrDefaultAsync();

                if (employee != null)
                {
                    response.Data = employee;
                    return response;
                }

                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = $"{ex.Message}: \n {ex.StackTrace}";
                return response;
            }
        }

        public async Task<Response<bool>> EditEmployeeByIdAsync(EmployeeDto employeeDto)
        {
            var response = new Response<bool>()
            {
                StatusCode = 200,
                Data = true,
                Succeeded = true
            };

            try
            {
                if(employeeDto == null)
                {
                    response.StatusCode = 400;
                    return response;
                }

                var employee = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == employeeDto.EmployeeId && x.IsActive);

                if (employee == null)
                {
                    response.Message = "Record not found";
                    return response;
                }

                employee.FirstName = !string.IsNullOrEmpty(employeeDto.FirstName) ? employeeDto.FirstName : employee.FirstName;
                employee.FirstName = !string.IsNullOrEmpty(employeeDto.LastName) ? employeeDto.LastName : employee.LastName;
                employee.FirstName = !string.IsNullOrEmpty(employeeDto.UserName) ? employeeDto.UserName : employee.UserName;
                employee.FirstName = !string.IsNullOrEmpty(employeeDto.PhoneNumber) ? employeeDto.PhoneNumber : employee.PhoneNumber;

                await _userManager.UpdateAsync(employee);
                await _dbContext.SaveChangesAsync();

                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = $"{ex.Message}: \n {ex.StackTrace}";
                return response;
            }
        }

        public async Task<Response<bool>> DeleteEmployeeAsync(string employeeId)
        {
            var response = new Response<bool>()
            {
                StatusCode = 200,
                Data = false,
                Succeeded = true
            };

            try
            {
                if(string.IsNullOrEmpty(employeeId))
                {
                    response.StatusCode = 400;
                    return response;
                }

                var employee = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == employeeId);

                if (employee == null)
                {
                    response.Message = "Record not found";
                    return response;
                }

                employee.IsActive = false;

                await _userManager.UpdateAsync(employee);
                await _dbContext.SaveChangesAsync();

                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = $"{ex.Message}: \n {ex.StackTrace}";
                response.Succeeded = false;
                return response;
            }
        }
    }
}
