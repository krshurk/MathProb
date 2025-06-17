using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    public class CompletedTask
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        
        [NotMapped]
        public User User { get; set; }
        
        public int TaskId { get; set; }
        
        [NotMapped]
        public Exercise Exercise { get; set; }
        
        public DateTime? CompletionDate { get; set; }
        public int Score { get; set; }
    }
} 