﻿using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.Handlers;
using Microsoft.AspNetCore.Identity;

namespace AbsenceManagementSystem.Core.IRepositories
{
    public interface IEmployeeRepository
    {
        Task<EmployeeDto> AddNewEmployeeAsync(EmployeeDto user);
        Task<List<EmployeeDto>> GetAllEmployeesAsync();
        Task<Response<EmployeeDto>> GetEmployeeByIdAsync(string employeeId);
        Task<Response<bool>> EditEmployeeByIdAsync(EmployeeDto employeeDto);
        Task<Response<bool>> DeleteEmployeeAsync(string employeeId);
    }
}
