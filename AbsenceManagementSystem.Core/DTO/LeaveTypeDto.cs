using AbsenceManagementSystem.Core.Domain;
using AbsenceManagementSystem.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace AbsenceManagementSystem.Core.DTO
{
    public class LeaveTypeDto
    {
        public LeaveTypes Type { get; set; }
        public int DefaultNumberOfDays { get; set; }
    }

    public class LeaveTypeResponseDto
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public int DefaultNumberOfDays { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class EmployeeLeaveRequestDto
    {
        [Required]
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfDaysOff { get; set; }
        public LeaveTypes LeaveType { get; set; }
    }

    public class EmployeeLeaveRequesResponsetDto
    {
        public string Id { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime RequestDate { get; set; }
        public int NumberOfDaysOff { get; set; }
        public LeaveTypes LeaveType { get; set; }
        public LeaveStatus Status { get; set; }
    }

    public class UpdateEmployeeLeaveRequesDto
    {
        public string Id { get; set; }
        public LeaveStatus Status { get; set; }
    }
}
