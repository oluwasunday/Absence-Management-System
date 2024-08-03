namespace AbsenceManagementSystem.Core.DTO
{
    public class LoginResponseDto
    {
        public string Id { get; set; }
        public string Token { get; set; }
        public Guid RefreshToken { get; set; }
    }
}
