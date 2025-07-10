
namespace BLL.ModelsDto
{
    public class ArticleDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public required string AuthorId { get; set; }
        public string AuthorName { get; set; } = null!;

        public int TagsCount { get; set; } 
        public int CommentsCount { get; set; } 
    }
}
