using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace WebApplication.Models
{
    [Table("Exam_questions")]
    public class ExamQuestion
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("question")]
        public string Question { get; set; }

        [Required]
        [Column("options")]
        public JsonDocument Options { get; set; }

        [Required]
        [Column("correct_answer")]
        [StringLength(10)]
        public string CorrectAnswer { get; set; }

        [Column("explanation")]
        public string Explanation { get; set; }
    }
} 