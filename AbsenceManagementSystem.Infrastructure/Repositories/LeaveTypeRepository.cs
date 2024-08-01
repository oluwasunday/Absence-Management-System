using AbsenceManagementSystem.Core.Domain;
using AbsenceManagementSystem.Core.IRepositories;
using AbsenceManagementSystem.Infrastructure.DbContext;
using AbsenceManagementSystem.Infrastructure.Repositories.Base;

namespace AbsenceManagementSystem.Infrastructure.Repositories
{
    public class LeaveTypeRepository : Repository<LeaveType>, ILeaveTypeRepository
    {
        private readonly AMSDbContext _dbContext;

        public LeaveTypeRepository(AMSDbContext dbContext) : base(dbContext) 
        {
            _dbContext = dbContext;
        }

    }
}
