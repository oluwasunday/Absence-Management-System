using AbsenceManagementSystem.Core.Domain;
using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.IRepositories.Base;
using Microsoft.AspNetCore.Identity;

namespace AbsenceManagementSystem.Core.IRepositories
{
    public interface IEmployeeRepository
    {
        Task<EmployeeDto> AddNewEmployeeAsync(EmployeeDto user);
        Task<List<EmployeeDto>> GetAllEmployeesAsync();
    }
}
