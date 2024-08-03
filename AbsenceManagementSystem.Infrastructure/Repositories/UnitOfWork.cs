using AbsenceManagementSystem.Core.IRepositories;
using AbsenceManagementSystem.Core.UnitOfWork;
using AbsenceManagementSystem.Infrastructure.DbContext;

namespace AbsenceManagementSystem.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public ILeaveTypeRepository LeaveTypes { get; set; }
        public IEmployeeLeaveRequestRepository EmployeeLeaveRequests { get; set; }
        private readonly AMSDbContext _context;

        public UnitOfWork(AMSDbContext context)
        {
            _context = context;
            LeaveTypes = new LeaveTypeRepository(_context);
            EmployeeLeaveRequests = new EmployeeLeaveRequestRepository(_context);
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
