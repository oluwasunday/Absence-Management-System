using AbsenceManagementSystem.Core.Domain;
using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.Handlers;
using AbsenceManagementSystem.Core.IServices;
using AbsenceManagementSystem.Core.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AbsenceManagementSystem.Services.Services
{
    public class LeaveTypeService : ILeaveTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LeaveTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<LeaveTypeResponseDto>> AddNewLeaveTypeAsync(LeaveTypeDto leaveDto)
        {
            try
            {
                if (leaveDto == null)
                {
                    throw new ArgumentNullException();
                }

                LeaveType leaveType = new LeaveType()
                {
                    Id = Guid.NewGuid().ToString(),
                    DateModified = DateTime.Now,
                    Type = leaveDto.Type,
                    DefaultNumberOfDays = leaveDto.DefaultNumberOfDays
                };


                await _unitOfWork.LeaveTypes.AddAsync(leaveType);
                await _unitOfWork.CompleteAsync();

                var leaveTypeResponse = new LeaveTypeResponseDto()
                {
                    Id = leaveType.Id,
                    DateCreated = leaveType.DateCreated,
                    DefaultNumberOfDays = leaveType.DefaultNumberOfDays,
                    Type = leaveDto.Type.ToString(),
                    IsActive = true
                };

                return new Response<LeaveTypeResponseDto>()
                {
                    StatusCode = StatusCodes.Status201Created,
                    Succeeded = true,
                    Data = leaveTypeResponse,
                    Message = $"Successfully added"
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
        }

        public async Task<Response<List<LeaveTypeResponseDto>>> GetAllLeaveTypesAsync()
        {
            try
            {
                var types = await _unitOfWork.LeaveTypes.GetAllAsQueryable().Where(x => x.IsActive && x.IsDeleted == false)
                    .Select(x => new LeaveTypeResponseDto()
                    {
                        Id = x.Id,
                        DateCreated = x.DateCreated,
                        Type = x.Type.ToString(),
                        DefaultNumberOfDays = x.DefaultNumberOfDays, 
                        IsActive = x.IsActive
                    }
                ).ToListAsync();

                return new Response<List<LeaveTypeResponseDto>>()
                {
                    StatusCode = StatusCodes.Status200OK,
                    Succeeded = true,
                    Data = types,
                    Message = $"Success!"
                };
            }
            catch (Exception ex)
            {
                return new Response<List<LeaveTypeResponseDto>>()
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Succeeded = false,
                    Data = null,
                    Message = $"{ex.Message} - {ex.StackTrace}"
                };
            }
        }

        public async Task<Response<LeaveTypeResponseDto>> GetLeaveTypeAsync(string typeId)
        {
            try
            {
                var leaveType = await _unitOfWork.LeaveTypes.GetAllAsQueryable().Where(x => x.Id == typeId && x.IsActive && !x.IsDeleted)
                    .Select(x => new LeaveTypeResponseDto()
                    {
                        Id = x.Id,
                        DateCreated = x.DateCreated,
                        Type = x.Type.ToString(),
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
        }
    }
}
