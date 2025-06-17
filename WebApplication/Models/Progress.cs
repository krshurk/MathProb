using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    [Table("Progresses")]
    public class Progress
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("completes_exercises")]
        public int CompletesExercises { get; set; }

        [Required]
        [Column("score")]
        public int Score { get; set; }

        [Required]
        [Column("course_completion")]
        public decimal CourseCompletion { get; set; }

        // Навигационное свойство
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}