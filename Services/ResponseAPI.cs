namespace DinamisAPI.Services
{
    public class ResponseAPI
    {
        public string? Message { get; set; }
        public string? Token { get; set; }
        public string? Status { get; set; }
        public bool Success { get; set; }
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }
    }
}
