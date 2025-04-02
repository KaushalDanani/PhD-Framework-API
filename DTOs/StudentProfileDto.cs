namespace Backend.DTOs
{
    public class StudentProfileDto : UserProfileDto
    {
        public string RegistrationNo { get; set; }
        public string DOB { get; set; }
    }
}
