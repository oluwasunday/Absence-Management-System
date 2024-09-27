using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.Enums;
using AbsenceManagementSystem.Core.Handlers;
using AbsenceManagementSystem.Core.IRepositories;
using AbsenceManagementSystem.Core.IServices;
using AbsenceManagementSystem.Core.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace AbsenceManagementSystem.Services.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public EmployeeService(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<Response<EmployeeDto>> AddNewEmployeeAsync(EmployeeDto user)
        {
            var result = await _employeeRepository.AddNewEmployeeAsync(user);

            if (result.Succeeded)
            {
                // send mail
                var mailPayload = new EmailRequestDto
                {
                    Subject = "Welcome Onbaord",
                    Body = $@"Hi {result.Data.FirstName}! <br>You have been added to Absence Management System. Pls find your login details below:<br>
                            Username: {result.Data.Email} <br>
                            Password: {user.Password}",
                    CcEmail = user.Email,
                    CcName = user.LastName,
                    ToEmail = user.Email,
                    ToName = user.FirstName
                };

                var sendMail = await _emailService.SendEmailAsync(mailPayload);
            }

            return result;
        }

        public async Task<Response<List<EmployeeDto>>> GetAllEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllEmployeesAsync();
            return employees;
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

        public async Task<Response<EmployeeDashboardDto>> EmployeeInfoForDashboard(string employeeId)
        {
            try
            {
                var response = new Response<EmployeeDashboardDto>()
                {
                    StatusCode = 200,
                    Succeeded = true
                };

                // get employee info
                var employeeInfo = await _employeeRepository.GetEmployeeByIdAsync(employeeId);
                // get employee info
                var employeeLeaveInfo = _unitOfWork.EmployeeLeaveRequests.GetAllAsQueryable();
                var employeeLeaveDetails = employeeLeaveInfo
                    .Where(x => x.EmployeeId == employeeId)
                    .Select(x => new EmployeeLeaveRequesResponsetDto
                    {
                        EmployeeId = x.EmployeeId,
                        Id = x.Id,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        Status = x.Status,
                        NumberOfDaysOff = x.NumberOfDaysOff,
                        RequestDate = x.DateCreated,
                        EmployeeName = x.EmployeeName,
                        LeaveType = x.LeaveType
                    })
                    .ToList();

                var totalLeaveEntitled = employeeInfo?.Data?.TotalHolidayEntitlement ?? 0;
                var totalRemaining = totalLeaveEntitled - employeeLeaveInfo.Where(x => x.EmployeeId == employeeId && x.IsDeleted == false && x.Status != LeaveStatus.Cancelled && x.Status != LeaveStatus.Rejected).Sum(x => x.NumberOfDaysOff);

                var result = new EmployeeDashboardDto
                {
                    TotalLeaveRemaining = totalRemaining,
                    LeaveRecords = employeeLeaveDetails
                };

                response.Data = result;
                return response;
            }
            catch (Exception ex)
            {
                return new Response<EmployeeDashboardDto> { StatusCode = 500, Data = null, Errors = ex.Message, Succeeded = false };
            }
            
        }

        public async Task<Response<AdminDashboard>> EmployeesInfoForAdminDashboard(string userId)
        {
            try
            {
                var response = new Response<AdminDashboard>()
                {
                    StatusCode = 200, Succeeded = true
                };

                var employees = await _employeeRepository.GetAllEmployeesCountAsync();
                var leaves = _unitOfWork.EmployeeLeaveRequests.GetAllAsQueryable().Where(x => x.IsActive && x.IsDeleted == false);
                var dashboard = new AdminDashboard
                {
                    UserId = userId,
                    NumberOfEmployees = employees.Data,
                    EmployeesOnSickLeave = leaves.Where(x => x.Status == LeaveStatus.Approved).Count(),
                    EmployeesOnCasualLeave = leaves.Where(x => x.Status == LeaveStatus.Approved).Count(),
                    PendingLeave = leaves.Where(x => x.Status == LeaveStatus.Pending).Count()
                };

                response.Data = dashboard;
                return response;
            }
            catch (Exception ex)
            {
                return new Response<AdminDashboard> { StatusCode = 500, Data = null, Errors = ex.Message, Succeeded = false };
            }
            
        }

        public async Task<Response<LeaveEntitlementDto>> EmployeeLeaveEntitlement(string employeeId)
        {
            try
            {
                var response = new Response<LeaveEntitlementDto>()
                {
                    StatusCode = 200,
                    Succeeded = true
                };

                // get employee info
                var employeeInfo = await _employeeRepository.GetEmployeeByIdAsync(employeeId);
                // get employee info
                var employeeLeaveInfo = _unitOfWork.EmployeeLeaveRequests.GetAllAsQueryable()
                    .Where(x => x.EmployeeId == employeeId && x.IsDeleted == false && x.Status != LeaveStatus.Cancelled && x.Status != LeaveStatus.Rejected);

                var leaveTaken = employeeLeaveInfo.Where(x => x.Status == LeaveStatus.Approved).Sum(x => x.NumberOfDaysOff);
                var totaleLeaveEntitledTo = employeeInfo.Data.TotalHolidayEntitlement;
                var totalPending = employeeLeaveInfo.Where(x => x.Status == LeaveStatus.Pending).Sum(x => x.NumberOfDaysOff);
                var balance = totaleLeaveEntitledTo - leaveTaken - totalPending;

                var leaveEntitlementDto = new LeaveEntitlementDto
                {
                    EmployeeName = employeeInfo.Data.FirstName + " " + employeeInfo.Data.LastName,
                    ContractType = employeeInfo.Data.ContractType.ToString(),
                    LeaveBalance = balance,
                    TotalLeaveEntitled = totaleLeaveEntitledTo,
                    TotalLeavePending = totalPending,
                    TotalLeaveTaken = leaveTaken
                };

                response.Data = leaveEntitlementDto;
                return response;
            }
            catch (Exception ex)
            {
                return new Response<LeaveEntitlementDto> { StatusCode = 500, Data = null, Errors = ex.Message, Succeeded = false };
            }
            
        }

        public async Task<bool> UpdateEmployeeTotalLeave(string employeeId, int leaveDays)
        {
            return await _employeeRepository.UpdateEmployeeTotalLeave(employeeId, leaveDays);
        }
    }
}
