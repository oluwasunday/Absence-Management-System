using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.Handlers;

namespace AbsenceManagementSystem.Core.IServices
{
    public interface IEmployeeService
    {
        Task<Response<EmployeeDto>> AddNewEmployeeAsync(EmployeeDto user);
        Task<Response<List<EmployeeDto>>> GetAllEmployeesAsync();
        Task<Response<EmployeeDto>> GetEmployeeByIdAsync(string employeeId);
        Task<Response<bool>> EditEmployeeByIdAsync(EmployeeDto employeeDto);
        Task<Response<bool>> DeleteEmployeeAsync(string employeeId);
        Task<Response<EmployeeDashboardDto>> EmployeeInfoForDashboard(string employeeId);
        Task<bool> UpdateEmployeeTotalLeave(string employeeId, int leaveDays);
        Task<Response<LeaveEntitlementDto>> EmployeeLeaveEntitlement(string employeeId);
    }
}
