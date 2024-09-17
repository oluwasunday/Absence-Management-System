using AbsenceManagementSystem.Core.Domain;

namespace AbsenceManagementSystem.Core.IServices
{
    public interface IAbsencePredictorService
    {
        //bool PredictAbsence(EmployeeLeaveRequest inputData);
        void TrainModelMain();
        //void TrainModel2();
        bool PredictAbsenceMain(EmployeeLeaveRequest inputData);
    }
}
