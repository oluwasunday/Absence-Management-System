using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.Handlers;
using Microsoft.AspNetCore.Identity;

namespace AbsenceManagementSystem.Core.IRepositories
{
    public interface IEmployeeRepository
    {
        Task<Response<EmployeeDto>> AddNewEmployeeAsync(EmployeeDto user);
        Task<Response<List<EmployeeDto>>> GetAllEmployeesAsync();
        Task<Response<EmployeeDto>> GetEmployeeByIdAsync(string employeeId);
        Task<Response<bool>> EditEmployeeByIdAsync(EmployeeDto employeeDto);
        Task<Response<bool>> DeleteEmployeeAsync(string employeeId);
        Task<bool> UpdateEmployeeTotalLeave(string employeeId, int leaveDaysToUpdate);
    }
}
