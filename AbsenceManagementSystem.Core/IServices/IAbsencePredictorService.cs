using AbsenceManagementSystem.Core.Domain;

namespace AbsenceManagementSystem.Core.IServices
{
    public interface IAbsencePredictorService
    {
        void TrainModelMain();
        bool PredictAbsenceMain(EmployeeLeaveRequest inputData);
        Task<List<EmployeeLeavePredictResponse>> PredictAbsences();
    }
}
