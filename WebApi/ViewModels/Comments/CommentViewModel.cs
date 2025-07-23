namespace WebApi.ViewModels.Comments
{
    public class CommentViewModel
    {
        public required string ArticleId { get; set; }
        public List<Comment> Comments { get; set; } = [];
    }
}
