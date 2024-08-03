using AbsenceManagementSystem.Core.Domain;
using AbsenceManagementSystem.Core.IRepositories;
using AbsenceManagementSystem.Infrastructure.DbContext;
using AbsenceManagementSystem.Infrastructure.Repositories.Base;

namespace AbsenceManagementSystem.Infrastructure.Repositories
{
    public class EmployeeLeaveRequestRepository : Repository<EmployeeLeaveRequest>, IEmployeeLeaveRequestRepository
    {
        private readonly AMSDbContext _dbContext;

        public EmployeeLeaveRequestRepository(AMSDbContext dbContext) : base(dbContext) 
        {
            _dbContext = dbContext;
        }

    }
}
