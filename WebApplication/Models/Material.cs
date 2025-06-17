using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace WebApplication.Models
{
    [Table("Learning_materials")]
    public class Material
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("topic_id")]
        public int TopicId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("title")]
        public string Title { get; set; }

        [Required]
        [Column("content")]
        public string Content { get; set; }

        [Required]
        [Column("type")]
        public int TypeId { get; set; }

        [ForeignKey("TopicId")]
        public virtual Topic Topic { get; set; }

        [ForeignKey("TypeId")]
        public virtual MaterialType Type { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
} 