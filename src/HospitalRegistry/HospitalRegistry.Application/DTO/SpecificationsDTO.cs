using HospitalRegistry.Application.Enums;

namespace HospitalRegistry.Application.DTO
{
    public class SpecificationsDTO
    {
        public int PageSize { get; set; } = 25;
        public int PageNumber { get; set; } = 1;
        public string? SortField { get; set; }
        public SortDirection? SortDirection { get; set; }
    }
}
