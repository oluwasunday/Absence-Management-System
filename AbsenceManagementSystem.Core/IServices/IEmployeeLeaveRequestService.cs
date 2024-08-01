using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.Handlers;

namespace AbsenceManagementSystem.Core.IServices
{
    public interface IEmployeeLeaveRequestService
    {
        Task<Response<EmployeeLeaveRequesResponsetDto>> AddNewLeaveRequestAsync(EmployeeLeaveRequestDto requestDto);
        Task<Response<List<EmployeeLeaveRequesResponsetDto>>> GetAllLeaveRequestsAsync();
    }
}
