using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public required Article Article { get; set; }
        [Required]
        public required User Author { get; set; }
    }
}
