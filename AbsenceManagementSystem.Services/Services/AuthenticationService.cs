using AbsenceManagementSystem.Core.Domain;
using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.Handlers;
using AbsenceManagementSystem.Core.IRepositories;
using AbsenceManagementSystem.Core.IServices;
using AbsenceManagementSystem.Core.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace AbsenceManagementSystem.Services.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Employee> _userManager;
        private readonly ITokenGeneratorService _tokenGenerator;
        private readonly ITokenRepository _tokenRepository;

        public AuthenticationService(IUnitOfWork unitOfWork, UserManager<Employee> userManager, ITokenGeneratorService tokenGenerator, ITokenRepository tokenRepository)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _tokenGenerator = tokenGenerator;
            _tokenRepository = tokenRepository;
        }

        public async Task<Response<LoginResponseDto>> Login(LoginDto loginDto)
        {
            var response = new Response<LoginResponseDto>();
            try
            {

                var validityResult = await ValidateUser(loginDto);

                if (!validityResult.Succeeded)
                {
                    response.Message = validityResult.Message;
                    response.StatusCode = validityResult.StatusCode;
                    response.Succeeded = false;
                    return response;
                }

                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                var refreshToken = _tokenGenerator.GenerateRefreshToken();
                user.RefreshToken = refreshToken.ToString();
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7); //sets refresh token for 7 days
                var result = new LoginResponseDto()
                {
                    Id = user.Id,
                    Token = await _tokenGenerator.GenerateToken(user),
                    RefreshToken = refreshToken
                };

                await _userManager.UpdateAsync(user);

                response.StatusCode = (int)HttpStatusCode.OK;
                response.Message = "Login Successful!";
                response.Data = result;
                response.Succeeded = true;
                return response;
            }
            catch (Exception ex)
            {
                response.Message = $"{ex.Message} - {ex.StackTrace}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Succeeded = false;
                return response;
            }
            
        }

        private async Task<Response<bool>> ValidateUser(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var response = new Response<bool>();
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                response.Message = "Invalid Credentials";
                response.Succeeded = false;
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                return response;
            }

            if (!await _userManager.IsEmailConfirmedAsync(user) && user.IsActive)
            {
                response.Message = "Account not activated";
                response.Succeeded = false;
                response.StatusCode = (int)HttpStatusCode.Forbidden;
                return response;
            }
            else
            {
                response.Succeeded = true;
                response.StatusCode = (int)HttpStatusCode.OK;
                return response;
            }
        }
    }
}
