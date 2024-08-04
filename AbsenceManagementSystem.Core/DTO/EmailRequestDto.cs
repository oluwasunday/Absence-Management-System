using Microsoft.AspNetCore.Http;

namespace AbsenceManagementSystem.Core.DTO
{
    public class EmailRequestDto
    {
        public string ToEmail { get; set; }
        public string ToName { get; set; }
        public string CcEmail { get; set; }
        public string CcName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        //public List<IFormFile> Attachments { get; set; }
    }
}
