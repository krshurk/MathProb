using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    [Table("Exercise_attempts")]
    public class ExerciseAttempt
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("exercise_id")]
        public int ExerciseId { get; set; }

        [Required]
        [Column("is_correct")]
        public bool IsCorrect { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("ExerciseId")]
        public virtual Exercise Exercise { get; set; }
    }
} 