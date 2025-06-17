using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    [Table("Certificates")]
    public class Certificate
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("exam_id")]
        public int ExamId { get; set; }

        [Required]
        [Column("issue_date")]
        public DateTime IssueDate { get; set; }

        [Required]
        [Column("certificate_code")]
        [StringLength(255)]
        public string CertificateCode { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("ExamId")]
        public Exam Exam { get; set; }
    }
} 