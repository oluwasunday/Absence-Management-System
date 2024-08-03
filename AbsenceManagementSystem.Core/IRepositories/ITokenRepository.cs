using AbsenceManagementSystem.Core.Domain;

namespace AbsenceManagementSystem.Core.IRepositories
{
    public interface ITokenRepository
    {
        Task<Employee> GetUserByRefreshToken(Guid token, string userId);
    }
}
