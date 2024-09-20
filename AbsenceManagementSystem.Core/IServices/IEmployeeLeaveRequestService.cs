using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.Enums;
using AbsenceManagementSystem.Core.Handlers;

namespace AbsenceManagementSystem.Core.IServices
{
    public interface IEmployeeLeaveRequestService
    {
        Task<Response<EmployeeLeaveRequesResponsetDto>> AddNewLeaveRequestAsync(EmployeeLeaveRequestDto requestDto);
        Task<Response<List<EmployeeLeaveRequesResponsetDto>>> GetAllLeaveRequestsAsync();
        Task<Response<List<EmployeeLeaveRequesResponsetDto>>> GetAllLeaveRequestsByEmployeeIdAsync(string employeeId);
        Task<Response<bool>> UpdateLeaveRequestStatusAsync(LeaveStatus status, string id);
        Task<Response<bool>> EditLeaveRequestStatusAsync(EmployeeLeaveRequestDto requestDto);
        Task<Response<bool>> DeleteLeaveRequestStatusAsync(string requestId);
        Task<Response<List<EmployeeLeaveRequesResponsetDto>>> GetAllPendingLeaveRequestsAsync();
    }
}
