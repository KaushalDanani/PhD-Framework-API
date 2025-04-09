namespace Backend.DTOs
{
    public class GuideProfileDto : UserProfileDto
    {
        public string DOB { get; set; }
        public string Designation { get; set; }
        public int StudentLimit { get; set; }
        public int ExperienceYears { get; set; }
        public string Specialist { get; set; }
    }
}
