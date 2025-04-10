namespace Backend.DTOs
{
    public class PhDTitleRegistrationRequestDto
    {
        public string PhDId { get; set; }
        public string PhdTitle { get; set; }
        public string ResearchArea { get; set; }
        public int GuideId { get; set; }
        public int CoGuideId { get; set; }
        public DateTime StartDate { get; set; }
    }
}
