using AbsenceManagementSystem.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace AbsenceManagementSystem.Core.Domain
{
    public class EmployeeLeaveRequest : BaseEntity
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfDaysOff{ get; set; }
        public LeaveTypes LeaveType { get; set; }
        public LeaveStatus Status { get; set; }
    }
}
