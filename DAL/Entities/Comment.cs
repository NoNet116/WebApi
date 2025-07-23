using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }

        [Required]
        public required string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public required int  ArticleId { get; set; }
        public required string AuthorId { get; set; }
        [Required]
        public required Article Article { get; set; }
        [Required]
        public required User Author { get; set; }

    }
}
