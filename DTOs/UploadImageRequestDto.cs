namespace Backend.DTOs
{
    public class UploadImageRequestDto
    {
        public required IFormFile Image { get; set; }
        public required string Email { get; set; }
    }
}
