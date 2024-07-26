using AbsenceManagementSystem.Core.DTO;

namespace AbsenceManagementSystem.Core.IServices
{
    public interface IEmployeeService
    {
        Task<EmployeeDto> AddNewEmployeeAsync(EmployeeDto user);
        Task<List<EmployeeDto>> GetAllEmployeesAsync();
    }
}
