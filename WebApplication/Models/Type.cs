using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    [Table("Types")]
    public class MaterialType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("name_of_type")]
        [StringLength(255)]
        public string NameOfType { get; set; }
    }
} 