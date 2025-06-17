using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    [Table("Comments")]
    public class Comment
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("learning_material_id")]
        public int LearningMaterialId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("text")]
        public string Text { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("LearningMaterialId")]
        public virtual Material LearningMaterial { get; set; }
    }
} 