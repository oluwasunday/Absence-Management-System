using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.IServices;
using Microsoft.AspNetCore.Mvc;

namespace AbsenceManagementSystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveTypesController : ControllerBase
    {
        private readonly ILeaveTypeService _leaveTypeService;

        public LeaveTypesController(ILeaveTypeService leaveTypeService)
        {
            _leaveTypeService = leaveTypeService;
        }

        // GET: LeaveRequestsController
        [HttpGet("")]
        public async Task<IActionResult> GetAllLeaveTypes()
        {
            var result = await _leaveTypeService.GetAllLeaveTypesAsync();
            return Ok(result);
        }

        // POST: LeaveRequestsController/Create
        [HttpPost("")]
        public async Task<IActionResult> AddNewLeaveTypes(LeaveTypeDto dto)
        {
            try
            {
                var result = await _leaveTypeService.AddNewLeaveTypeAsync(dto);
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
