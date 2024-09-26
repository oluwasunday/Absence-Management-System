using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AbsenceManagementSystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private string _userId;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;            
        }

        // GET: EmployeesController
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetAllEmployes()
        {
            var result = await _employeeService.GetAllEmployeesAsync();
            return Ok(result);
        }

        // POST: EmployeesController/Create
        [HttpPost("")]
        public async Task<IActionResult> AddNewEmployee(EmployeeDto employee)
        {
            try
            {
                var result = await _employeeService.AddNewEmployeeAsync(employee);
                return Ok(result);
            }
            catch
            {
                return BadRequest(employee);
            }
        }

        // GET: EmployeesController
        [HttpGet("{employeeid}")]
        public async Task<IActionResult> GetEmployeeById(string employeeid)
        {
            var result = await _employeeService.GetEmployeeByIdAsync(employeeid);
            return Ok(result);
        }

        // PUT: EmployeesController
        [HttpPut("")]
        public async Task<IActionResult> EditEmployee(EmployeeDto employeeDto)
        {
            var result = await _employeeService.EditEmployeeByIdAsync(employeeDto);
            return Ok(result);
        }

        // DELETE: EmployeesController
        [HttpDelete("")]
        public async Task<IActionResult> DeleteEmployee(string employeeId)
        {
            string userId = HttpContext.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var result = await _employeeService.DeleteEmployeeAsync(employeeId);
            return Ok(result);
        }

        // GET: EmployeesleaveDashboard
        [HttpGet("employeedashboard")]
        public async Task<IActionResult> EmployeeInfoForDashboard()
        {
            string employeeId = HttpContext.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var result = await _employeeService.EmployeeInfoForDashboard(employeeId);
            return Ok(result);
        }

        // GET: EmployeesleaveDashboard
        [HttpGet("employeeleaveentitlement")]
        public async Task<IActionResult> EmployeeLeaveEntitlement()
        {
            string employeeId = HttpContext.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var result = await _employeeService.EmployeeLeaveEntitlement(employeeId);
            return Ok(result);
        }

        // GET: EmployeesleaveDashboard
        [HttpGet("employeesinfoforadmindashboard/counts")]
        public async Task<IActionResult> EmployeesInfoForAdminDashboard()
        {
            _userId = HttpContext.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var result = await _employeeService.EmployeesInfoForAdminDashboard(_userId);
            return Ok(result);
        }
    }
}
