using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.Handlers;
using AbsenceManagementSystem.Core.IRepositories;
using AbsenceManagementSystem.Core.IServices;

namespace AbsenceManagementSystem.Services.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<EmployeeDto> AddNewEmployeeAsync(EmployeeDto user)
        {
            return await _employeeRepository.AddNewEmployeeAsync(user);
        }

        public async Task<List<EmployeeDto>> GetAllEmployeesAsync()
        {
            return await _employeeRepository.GetAllEmployeesAsync();
        }

        public async Task<Response<EmployeeDto>> GetEmployeeByIdAsync(string employeeId)
        {
            return await _employeeRepository.GetEmployeeByIdAsync(employeeId);
        }

        public async Task<Response<bool>> EditEmployeeByIdAsync(EmployeeDto employeeDto)
        {
            return await _employeeRepository.EditEmployeeByIdAsync(employeeDto);
        }

        public async Task<Response<bool>> DeleteEmployeeAsync(string employeeId)
        {
            return await _employeeRepository.DeleteEmployeeAsync(employeeId);
        }
    }
}
