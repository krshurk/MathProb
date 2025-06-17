using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    [Table("Topics")]
    public class Topic
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("title")]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        public List<Material> Materials { get; set; }

        public ICollection<Exercise> Exercises { get; set; }
    }
} 