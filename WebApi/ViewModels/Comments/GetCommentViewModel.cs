namespace WebApi.ViewModels.Comments
{
    public class GetCommentViewModel
    {
        public required string ArticleId { get; set; }

        public int Count { get; set; }
    }
}
