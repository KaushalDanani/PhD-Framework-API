using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend.Entities
{
    public class ProgressReport
    {
        [Required]
        public string RegistrationId { get; set; }
        [Required]
        public int ProgressReportNo { get; set; }

        [Required]
        public Guid FileId { get; set; }  // Stores reference to the actual file

        [Required]
        public DateTime SubmissionDate { get; set; }

        public bool GuideStatus { get; set; }  // True = Approved, False = Rejected/Pending
        public string? GuideRemark { get; set; }
        public DateTime? GuideReviewDate { get; set; }

        public bool DeanStatus { get; set; }
        public string? DeanRemark { get; set; }
        public DateTime? DeanReviewDate { get; set; }

        public bool AcademicSectionStatus { get; set; }
        public DateTime? AcademicSectionApproveDate { get; set; }
        public string? AcademicSectionRemark { get; set; }

        [Required]
        public bool ReportStatus { get; set; }  // True = Active, False = Rejected/Closed/Inactive

        public DateTime LastUpdatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("RegistrationId")]
        public virtual Student Student { get; set; }

        [ForeignKey("FileId")]
        public virtual ApplicationFile ApplicationFile { get; set; }
    }

}
