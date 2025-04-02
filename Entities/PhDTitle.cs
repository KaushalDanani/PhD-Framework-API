using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Entities
{
    public class PhDTitle
    {
        [Key, ForeignKey("Student")]
        public string RegistrationId { get; set; }

        [Required, ForeignKey("Guide")]
        public int GuideId { get; set; }

        [ForeignKey("CoGuide")]
        public int? CoGuideId { get; set; }  // Nullable, since not all PhD titles may have a co-guide

        [Required, MaxLength(275)]
        public string PhDTitleName { get; set; }

        [Required, MaxLength(255)]
        public string ResearchArea { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime LastUpdatedAt { get; set; } = DateTime.Now;

        [Required]
        public bool Status { get; set; }  // True for active, False for inactive

        public DateTime? ReRegistrationDate { get; set; }  // Nullable, some may not need re-registration

        public int Year { get; set; }

        public int Term { get; set; }

        // Navigation Properties
        public virtual Guide Guide { get; set; }

        public virtual Guide CoGuide { get; set; }

        public Student Student { get; set; }
    }

}
