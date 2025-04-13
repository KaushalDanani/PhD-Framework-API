namespace Backend.DTOs
{
    public class ReportKeyDto
    {
        public string RegistrationId { get; set; }
        public int ReportNo { get; set; }
    }

    public class ProgressReportReviewDto
    {
        public List<ReportKeyDto> Reports { get; set; }
        public bool Action { get; set; }
        public string? Remark { get; set; }
    }

}
