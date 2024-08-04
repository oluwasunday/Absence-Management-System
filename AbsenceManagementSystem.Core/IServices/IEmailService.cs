using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.Handlers;

namespace AbsenceManagementSystem.Core.IServices
{
    public interface IEmailService
    {
        Task<Response<string>> SendEmailAsync(EmailRequestDto mailRequest);
    }
}
