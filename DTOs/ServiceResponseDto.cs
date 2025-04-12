namespace Backend.DTOs
{
    public class ServiceResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }
    }
}
