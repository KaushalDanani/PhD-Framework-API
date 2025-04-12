namespace Backend.DTOs
{
    public class ProgressReportResponseDto
    {
        public int ProgressReportNo { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string FilePath { get; set; }

        public string SubmissionDate { get; set; }

        public bool GuideStatus { get; set; }
        public string? GuideRemark { get; set; }
        public string? GuideReviewDate { get; set; }

        public bool DeanStatus { get; set; }
        public string? DeanRemark { get; set; }
        public string? DeanReviewDate { get; set; }

        public bool AcademicSectionStatus { get; set; }
        public string? AcademicSectionApproveDate { get; set; }
        public string? AcademicSectionRemark { get; set; }

        public bool ReportStatus { get; set; }
        public string LastUpdatedAt { get; set; }
    }
}
