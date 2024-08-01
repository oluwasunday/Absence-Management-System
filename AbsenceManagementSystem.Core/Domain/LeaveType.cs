using AbsenceManagementSystem.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace AbsenceManagementSystem.Core.Domain
{
    public class LeaveType : BaseEntity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public LeaveTypes Type { get; set; }
        public int DefaultNumberOfDays { get; set; }
    }
}
