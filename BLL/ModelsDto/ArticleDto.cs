
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

        // Для тегов можно использовать:
        public List<TagDto> Tags { get; set; } = new List<TagDto>();

        // Для комментариев:
        public List<CommentDto> Comments { get; set; } = new List<CommentDto>();
    }
}
