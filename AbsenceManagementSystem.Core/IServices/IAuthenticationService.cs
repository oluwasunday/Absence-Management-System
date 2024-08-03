using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.Handlers;

namespace AbsenceManagementSystem.Core.IServices
{
    public interface IAuthenticationService
    {
        Task<Response<LoginResponseDto>> Login(LoginDto loginDto);
    }
}
