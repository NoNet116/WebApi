namespace WebApi.ViewModels.Comments
{
    public class GetCommentViewModel
    {
        public required int ArticleId { get; set; }

        public int Count { get; set; }
    }
}
