using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.Handlers;

namespace AbsenceManagementSystem.Core.IServices
{
    public interface ILeaveTypeService
    {
        Task<Response<LeaveTypeResponseDto>> AddNewLeaveTypeAsync(LeaveTypeDto leaveDto);
        Task<Response<List<LeaveTypeResponseDto>>> GetAllLeaveTypesAsync();
        Task<Response<LeaveTypeResponseDto>> GetLeaveTypeAsync(string typeId);
    }
}
