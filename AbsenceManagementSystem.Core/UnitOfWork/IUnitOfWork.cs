using AbsenceManagementSystem.Core.IRepositories;

namespace AbsenceManagementSystem.Core.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ILeaveTypeRepository LeaveTypes { get; }
        IEmployeeLeaveRequestRepository EmployeeLeaveRequests { get; }
        Task CompleteAsync();
    }
}
