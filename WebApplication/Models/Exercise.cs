using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace WebApplication.Models
{
    [Table("Exercises")]
    public class Exercise
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("topic_id")]
        public int TopicId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("question")]
        public string Question { get; set; }

        [Required]
        [StringLength(255)]
        [Column("correct_answer")]
        public string CorrectAnswer { get; set; }

        [Required]
        [StringLength(255)]
        [Column("description_correct_answer")]
        public string DescriptionCorrectAnswer { get; set; }

        [Required]
        [Column("options")]
        public JsonDocument Options { get; set; }

        [ForeignKey("TopicId")]
        public virtual Topic Topic { get; set; }
    }
} 