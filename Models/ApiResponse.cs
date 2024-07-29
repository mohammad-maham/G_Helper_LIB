namespace GoldHelpers.Models
{
    public class ApiResponse
    {
        public int? StatusCode { get; set; } = 200;
        public string? Data { get; set; }
        public string? Message { get; set; }
    }
}
