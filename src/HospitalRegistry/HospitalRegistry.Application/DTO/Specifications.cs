using HospitalRegistry.Application.Enums;

namespace HospitalRegistry.Application.DTO
{
    /// <summary>
    /// DTO for specification for read list of object request.
    /// </summary>
    public class Specifications
    {
        // Searching
        public string? SearchTerm { get; set; }

        // Pagination
        public int PageSize { get; set; } = 25;
        public int PageNumber { get; set; } = 1;

        // Sorting
        public string SortField { get; set; } = "Id";
        public SortDirection SortDirection { get; set; } = SortDirection.ASC;

        // Filtering 
        public IDictionary<string, string> Filters { get; set; } = new Dictionary<string, string>();
    }
}
