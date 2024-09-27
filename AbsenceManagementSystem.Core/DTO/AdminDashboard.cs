namespace AbsenceManagementSystem.Core.DTO
{
    public class AdminDashboard
    {
        public string UserId { get; set; }
        public int NumberOfEmployees { get; set; }
        public int EmployeesOnCasualLeave { get; set; }
        public int EmployeesOnSickLeave { get; set; }
        public int PendingLeave { get; set; }
    }
}
