using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AbsenceManagementSystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // GET: EmployeesController
        [HttpGet("")]
        public async Task<IActionResult> GetAllEmployes()
        {
            var result = await _employeeService.GetAllEmployeesAsync();
            return Ok(result);
        }

        // POST: EmployeesController/Create
        [HttpPost("")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNewEmployee(EmployeeDto employee)
        {
            try
            {
                var result = await _employeeService.AddNewEmployeeAsync(employee);
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }

        
    }
}
