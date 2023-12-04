namespace HospitalRegistry.Application.DTO
{
    public class ListModel<T>
    {
        public List<T> List { get; set; }
        public int TotalPages { get; set; }
    }
}
