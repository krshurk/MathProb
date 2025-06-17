using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    [Table("Exams")]
    public class Exam
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("score")]
        public int Score { get; set; }

        [Required]
        [Column("taken_at")]
        public DateTime TakenAt { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
} 