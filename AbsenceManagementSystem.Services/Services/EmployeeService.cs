using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.Handlers;
using AbsenceManagementSystem.Core.IRepositories;
using AbsenceManagementSystem.Core.IServices;
using AbsenceManagementSystem.Core.UnitOfWork;
using Microsoft.AspNetCore.Http;

namespace AbsenceManagementSystem.Services.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeService(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<EmployeeDto>> AddNewEmployeeAsync(EmployeeDto user)
        {
            return await _employeeRepository.AddNewEmployeeAsync(user);
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
                var employeeLeaveInfo = _unitOfWork.EmployeeLeaveRequests.GetAllAsQueryable()
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

                var result = new EmployeeDashboardDto
                {
                    TotalLeaveRemaining = totalLeaveEntitled,
                    LeaveRecords = employeeLeaveInfo
                };

                response.Data = result;
                return response;
            }
            catch (Exception ex)
            {
                return new Response<EmployeeDashboardDto> { StatusCode = 500, Data = null, Errors = ex.Message, Succeeded = false };
            }
            
        }

        public async Task<bool> UpdateEmployeeTotalLeave(string employeeId, int leaveDays)
        {
            return await _employeeRepository.UpdateEmployeeTotalLeave(employeeId, leaveDays);
        }
    }
}
