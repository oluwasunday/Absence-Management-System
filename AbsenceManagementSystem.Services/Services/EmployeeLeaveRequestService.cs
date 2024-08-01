using AbsenceManagementSystem.Core.Domain;
using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.Enums;
using AbsenceManagementSystem.Core.Handlers;
using AbsenceManagementSystem.Core.IServices;
using AbsenceManagementSystem.Core.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AbsenceManagementSystem.Services.Services
{
    public class EmployeeLeaveRequestService : IEmployeeLeaveRequestService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeLeaveRequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<EmployeeLeaveRequesResponsetDto>> AddNewLeaveRequestAsync(EmployeeLeaveRequestDto requestDto)
        {
            try
            {
                if (requestDto == null)
                {
                    throw new ArgumentNullException();
                }

                EmployeeLeaveRequest leaveRequest = new EmployeeLeaveRequest()
                {
                    Id = Guid.NewGuid().ToString(),
                    DateModified = DateTime.Now,
                    EmployeeId = requestDto.EmployeeId,
                    StartDate = requestDto.StartDate,
                    EndDate = requestDto.EndDate,
                    EmployeeName = requestDto.EmployeeName,
                    DateCreated = DateTime.Now,
                    Status = LeaveStatus.Pending,
                    IsActive = true,
                    IsDeleted = false,
                    LeaveType = requestDto.LeaveType,
                    NumberOfDaysOff = requestDto.NumberOfDaysOff
                };


                await _unitOfWork.EmployeeLeaveRequests.AddAsync(leaveRequest);
                await _unitOfWork.CompleteAsync();

                var leaveRequestResponse = new EmployeeLeaveRequesResponsetDto()
                {
                    Id = leaveRequest.Id,
                    StartDate = leaveRequest.StartDate,
                    EndDate = leaveRequest.EndDate,
                    NumberOfDaysOff = leaveRequest.NumberOfDaysOff,
                    Status = leaveRequest.Status,
                    RequestDate = leaveRequest.DateCreated,
                    EmployeeId = leaveRequest.EmployeeId,
                    EmployeeName= leaveRequest.EmployeeName,
                    LeaveType= leaveRequest.LeaveType
                };

                return new Response<EmployeeLeaveRequesResponsetDto>()
                {
                    StatusCode = StatusCodes.Status201Created,
                    Succeeded = true,
                    Data = leaveRequestResponse,
                    Message = $"Successfully added"
                };
            }
            catch (Exception ex)
            {
                return new Response<EmployeeLeaveRequesResponsetDto>()
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Succeeded = false,
                    Data = null,
                    Message = $"{ex.Message} - {ex.StackTrace}"
                };
            }
        }

        public async Task<Response<List<EmployeeLeaveRequesResponsetDto>>> GetAllLeaveRequestsAsync()
        {
            try
            {
                var leaveRequests = await _unitOfWork.EmployeeLeaveRequests.GetAllAsQueryable().Where(x => x.IsActive && !x.IsDeleted)
                    .Select(x => new EmployeeLeaveRequesResponsetDto()
                    {
                        Id = x.Id,
                        StartDate = x.StartDate,
                        RequestDate = x.DateCreated,
                        EmployeeName = x.EmployeeName,
                        EmployeeId = x.EmployeeId,
                        EndDate = x.EndDate,
                        NumberOfDaysOff = x.NumberOfDaysOff,
                        LeaveType = x.LeaveType,
                        Status = x.Status
                    }
                ).ToListAsync();

                return new Response<List<EmployeeLeaveRequesResponsetDto>>()
                {
                    StatusCode = StatusCodes.Status200OK,
                    Succeeded = true,
                    Data = leaveRequests,
                    Message = $"Success!"
                };
            }
            catch (Exception ex)
            {
                return new Response<List<EmployeeLeaveRequesResponsetDto>>()
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Succeeded = false,
                    Data = null,
                    Message = $"{ex.Message} - {ex.StackTrace}"
                };
            }
        }

        /*public async Task<Response<LeaveTypeResponseDto>> GetLeaveTypeAsync(string typeId)
        {
            try
            {
                var leaveType = await _unitOfWork.LeaveTypes.GetAllAsQueryable().Where(x => x.Id == typeId && x.IsActive && !x.IsDeleted)
                    .Select(x => new LeaveTypeResponseDto()
                    {
                        Id = x.Id,
                        DateCreated = x.DateCreated,
                        Type = x.Type,
                        DefaultNumberOfDays = x.DefaultNumberOfDays
                    }
                ).FirstOrDefaultAsync();

                return new Response<LeaveTypeResponseDto>()
                {
                    StatusCode = StatusCodes.Status200OK,
                    Succeeded = true,
                    Data = leaveType,
                    Message = $"Success!"
                };
            }
            catch (Exception ex)
            {
                return new Response<LeaveTypeResponseDto>()
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Succeeded = false,
                    Data = null,
                    Message = $"{ex.Message} - {ex.StackTrace}"
                };
            }
        }*/
    }
}
