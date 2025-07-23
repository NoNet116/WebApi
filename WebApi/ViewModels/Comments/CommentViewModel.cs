namespace WebApi.ViewModels.Comments
{
    public class CommentViewModel
    {
        public required int ArticleId { get; set; }
        public List<Comment> Comments { get; set; } = [];
    }
}
