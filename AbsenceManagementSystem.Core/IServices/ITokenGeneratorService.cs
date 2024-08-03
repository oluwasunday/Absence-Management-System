using AbsenceManagementSystem.Core.Domain;

namespace AbsenceManagementSystem.Core.IServices
{
    public interface ITokenGeneratorService
    {
        Guid GenerateRefreshToken();
        Task<string> GenerateToken(Employee user);
    }
}
