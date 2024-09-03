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
        private readonly IEmployeeService _employeeService;

        public EmployeeLeaveRequestService(IUnitOfWork unitOfWork, IEmployeeService employeeService)
        {
            _unitOfWork = unitOfWork;
            _employeeService = employeeService;
        }

        public async Task<Response<EmployeeLeaveRequesResponsetDto>> AddNewLeaveRequestAsync(EmployeeLeaveRequestDto requestDto)
        {
            try
            {
                var current = DateTime.Now;
                var startDayIn = new DateTime(current.Year, 1, 1);

                if (requestDto == null)
                {
                    throw new ArgumentNullException();
                }

                //get employee
                var employee = await _employeeService.GetEmployeeByIdAsync(requestDto.EmployeeId);
                if (requestDto == null)
                {
                    return new Response<EmployeeLeaveRequesResponsetDto>()
                    {
                        Errors = $"Employee with name {requestDto.EmployeeName} not found",
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound,
                        Succeeded = false
                    };
                }

                // confirm eligibility
                var employeeLeaveRequestsInCurrentyear = _unitOfWork.EmployeeLeaveRequests
                                                        .GetAllAsQueryable()
                                                        .Where(x => x.Id == requestDto.EmployeeId && x.StartDate >= startDayIn && x.EndDate <= current);

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

        public async Task<Response<List<EmployeeLeaveRequesResponsetDto>>> GetAllLeaveRequestsByEmployeeIdAsync(string employeeId)
        {
            try
            {
                var leaveRequests = await _unitOfWork.EmployeeLeaveRequests.GetAllAsQueryable().Where(x => x.EmployeeId == employeeId && x.IsActive && !x.IsDeleted)
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

        public async Task<Response<bool>> UpdateLeaveRequestStatusAsync(LeaveStatus status, string id)
        {
            try
            {
                var leaveRequest = await _unitOfWork.EmployeeLeaveRequests.GetAllAsQueryable().FirstOrDefaultAsync(x => x.Id == id && x.IsActive && !x.IsDeleted);

                leaveRequest.Status = status;
                leaveRequest.DateModified = DateTime.Now;

                _unitOfWork.EmployeeLeaveRequests.Update(leaveRequest);
                await _unitOfWork.CompleteAsync();

                return new Response<bool>()
                {
                    StatusCode = StatusCodes.Status200OK,
                    Succeeded = true,
                    Data = true,
                    Message = $"Success!"
                };
            }
            catch (Exception ex)
            {
                return new Response<bool>()
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Succeeded = false,
                    Data = false,
                    Message = $"{ex.Message} - {ex.StackTrace}"
                };
            }
        }

        public async Task<Response<bool>> EditLeaveRequestStatusAsync(EmployeeLeaveRequestDto requestDto)
        {
            try
            {
                var leaveRequest = await _unitOfWork.EmployeeLeaveRequests.GetAllAsQueryable().FirstOrDefaultAsync(x => x.Id == requestDto.EmployeeId && x.IsActive && !x.IsDeleted);

                if (leaveRequest == null)
                {
                    return new Response<bool>()
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Succeeded = true,
                        Data = false,
                        Message = $"Not found!"
                    };
                }

                leaveRequest.LeaveType = requestDto.LeaveType;
                leaveRequest.NumberOfDaysOff = requestDto.NumberOfDaysOff;
                leaveRequest.StartDate = requestDto.StartDate;
                leaveRequest.EndDate = requestDto.EndDate;
                leaveRequest.DateModified = DateTime.Now;

                _unitOfWork.EmployeeLeaveRequests.Update(leaveRequest);
                await _unitOfWork.CompleteAsync();

                return new Response<bool>()
                {
                    StatusCode = StatusCodes.Status200OK,
                    Succeeded = true,
                    Data = true,
                    Message = $"Success!"
                };
            }
            catch (Exception ex)
            {
                return new Response<bool>()
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Succeeded = false,
                    Data = false,
                    Message = $"{ex.Message} - {ex.StackTrace}"
                };
            }
        }

        public async Task<Response<bool>> DeleteLeaveRequestStatusAsync(string requestId)
        {
            try
            {
                var leaveRequest = await _unitOfWork.EmployeeLeaveRequests.GetAllAsQueryable().FirstOrDefaultAsync(x => x.Id == requestId && x.IsActive && !x.IsDeleted);

                if(leaveRequest == null)
                {
                    return new Response<bool>()
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Succeeded = true,
                        Data = false,
                        Message = $"Not found or already deleted!"
                    };
                }

                leaveRequest.IsActive = false;
                leaveRequest.IsDeleted = true;
                leaveRequest.DateModified = DateTime.Now;

                _unitOfWork.EmployeeLeaveRequests.Update(leaveRequest);
                await _unitOfWork.CompleteAsync();

                return new Response<bool>()
                {
                    StatusCode = StatusCodes.Status200OK,
                    Succeeded = true,
                    Data = true,
                    Message = $"Success!"
                };
            }
            catch (Exception ex)
            {
                return new Response<bool>()
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Succeeded = false,
                    Data = false,
                    Message = $"{ex.Message} - {ex.StackTrace}"
                };
            }
        }
    }
}
