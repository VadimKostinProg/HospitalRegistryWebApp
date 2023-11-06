namespace HospitalRegistry.Application.DTO
{
    public class AuthenticationResponse
    {
        public Guid UserId { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
