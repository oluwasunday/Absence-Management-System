using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.Enums;
using AbsenceManagementSystem.Core.IServices;
using AbsenceManagementSystem.Infrastructure.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AbsenceManagementSystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly IEmployeeLeaveRequestService _leaveRequestService;

        public LeaveRequestsController(IEmployeeLeaveRequestService leaveRequestService)
        {
            _leaveRequestService = leaveRequestService;
        }

        // GET: LeaveRequestsController
        [HttpGet("")]
        public async Task<IActionResult> GetAllLeaveRequest()
        {
            var result = await _leaveRequestService.GetAllLeaveRequestsAsync();
            return Ok(result);
        }

        // GET: LeaveRequestsController/employeeId
        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetAllLeaveRequest(string employeeId)
        {
            var result = await _leaveRequestService.GetAllLeaveRequestsByEmployeeIdAsync(employeeId);
            return Ok(result);
        }

        // POST: LeaveRequestsController/Create
        [HttpPost("")]
        public async Task<IActionResult> AddNewLeaveRequests(EmployeeLeaveRequestDto requestDto)
        {
            try
            {
                var result = await _leaveRequestService.AddNewLeaveRequestAsync(requestDto);
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }

        // PUT: LeaveRequestsController/Edit
        [HttpPut]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> EditLeaveRequests(EmployeeLeaveRequestDto requestDto)
        {
            try
            {
                var result = await _leaveRequestService.EditLeaveRequestStatusAsync(requestDto);
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }

        // PATCH: LeaveRequestsController/Patch
        [HttpPatch]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateLeaveRequests(UpdateEmployeeLeaveRequesDto requesDto)
        {
            try
            {
                var result = await _leaveRequestService.UpdateLeaveRequestStatusAsync(requesDto.Status, requesDto.Id);
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }

        // DELETE: LeaveRequestsController/Delete
        [HttpDelete("{requesId}")]
        [Authorize]
        public async Task<IActionResult> DeleteLeaveRequests(string requesId)
        {
            try
            {
                var result = await _leaveRequestService.DeleteLeaveRequestStatusAsync(requesId);
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }

        // GET: PendingLeaveRequestsController
        [HttpGet("allpendingrequests")]
        public async Task<IActionResult> GetAllPendingRequest()
        {
            var result = await _leaveRequestService.GetAllPendingLeaveRequestsAsync();
            return Ok(result);
        }
    }
}
