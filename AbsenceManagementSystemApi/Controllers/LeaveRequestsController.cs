using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.IServices;
using Microsoft.AspNetCore.Mvc;

namespace AbsenceManagementSystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
    }
}
