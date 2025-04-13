namespace Backend.DTOs
{
    public class ProgressReportRequestsMetaDataDto
    {
        public string ProfileImageUrl { get; set; }
        public bool isChecked { get; set; } = false;
        public string StudentName { get; set; }
        public string RegistrationId { get; set; }
        public string Title { get; set; }
        public string ResearchArea { get; set; }
        public string ReportFileName { get; set; }
        public string ReportFileUrl { get; set; }
        public string SubmittedOn { get; set; }
    }
}
