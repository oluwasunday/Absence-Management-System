using AbsenceManagementSystem.Core.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AbsenceManagementSystem.Core.Domain
{
    public class Employee : IdentityUser
    {
        [Key]
        public string Id { get; set; }
        [MaxLength(50)]
        [Required]
        public string FirstName { get; set; }
        [MaxLength(50)]
        [Required]
        public string LastName { get; set; }
        public string Gender { get; set; }
        public ContractType ContractType { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; }
        public int TotalHolidayEntitlement { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateModified { get; set; }
        public string RefreshToken { get; set; }
    }
}
