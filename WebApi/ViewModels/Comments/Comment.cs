namespace WebApi.ViewModels.Comments
{
    public class Comment
    {
        public required string Id { get; set; }
        public required string Message { get; set; }
        public required string Author { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
    }
}
