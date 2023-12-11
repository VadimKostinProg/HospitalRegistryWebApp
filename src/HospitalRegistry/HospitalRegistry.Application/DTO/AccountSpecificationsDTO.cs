namespace HospitalRegistry.Application.DTO
{
    public class AccountSpecificationsDTO : SpecificationsDTO
    {
        public string? SearchTerm { get; set; }
        public string Role { get; set; }
    }
}
