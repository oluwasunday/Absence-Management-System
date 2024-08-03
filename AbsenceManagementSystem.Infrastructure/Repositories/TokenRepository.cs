using AbsenceManagementSystem.Core.Domain;
using AbsenceManagementSystem.Core.IRepositories;
using AbsenceManagementSystem.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace AbsenceManagementSystem.Infrastructure.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly AMSDbContext _context;
        public TokenRepository(AMSDbContext context)
        {
            _context = context;
        }


        public async Task<Employee> GetUserByRefreshToken(Guid token, string userId)
        {
            //Check for user Id

            var user = await _context.Employees.SingleOrDefaultAsync(u => u.RefreshToken == token.ToString() && u.Id == userId);

            if (user == null)
            {
                throw new ArgumentException($"User with Id {userId} does not exist");
            }

            return user;
        }
    }
}
