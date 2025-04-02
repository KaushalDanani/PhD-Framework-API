using System.ComponentModel.DataAnnotations;

namespace Backend.Entities
{
    public class ApplicationFile
    {
        [Key]
        public Guid FileId { get; set; }  // Unique identifier for files

        [Required]
        public string RelatedTable { get; set; }  // Stores reference context (e.g., "ProgressReports")

        [Required, MaxLength(255)]
        public string FileName { get; set; }

        [Required]
        public string mimeType { get; set; }

        public long FileSize { get; set; }  // Store in bytes

        [Required]
        public string FilePath { get; set; }  // Path to the stored file

        [Required]
        public DateTime UploadedAt { get; set; } = DateTime.Now;
    }

}
