using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [Column("email")]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [Column("password_hash")]
        [StringLength(255)]
        public string PasswordHash { get; set; }


        public User()
        {
            Name = string.Empty;
            Email = string.Empty;
            PasswordHash = string.Empty;
        }
    }
} 