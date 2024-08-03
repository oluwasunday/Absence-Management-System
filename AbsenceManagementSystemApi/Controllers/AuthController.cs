using AbsenceManagementSystem.Core.Domain;
using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.Handlers;
using AbsenceManagementSystem.Core.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AbsenceManagementSystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly UserManager<Employee> _userManager;

        public AuthController(IAuthenticationService authenticationService, UserManager<Employee> userManager)
        {
            _authenticationService = authenticationService;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Response<string>>> Login([FromBody] LoginDto model)
        {
            //_logger.Information($"Login Attempt for {model.Email}");
            var result = await _authenticationService.Login(model);
            return StatusCode(result.StatusCode, result);
        }
    }
}
