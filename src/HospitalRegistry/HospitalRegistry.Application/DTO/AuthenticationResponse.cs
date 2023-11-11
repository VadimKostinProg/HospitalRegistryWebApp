namespace HospitalRegistry.Application.DTO
{
    /// <summary>
    /// DTO for 
    /// </summary>
    public class AuthenticationResponse
    {
        public string Token { get; set; }
        public string Role { get; set; }

        /// <summary>
        /// Id of doctor or patient record, null - for admin and recipients.
        /// </summary>
        public Guid? UserId { get; set; }
    }
}
