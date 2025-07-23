using System.ComponentModel.DataAnnotations;

namespace WebApi.ViewModels.Comments
{
    public class CreateCommentViewModel
    {
        public required int ArticleId { get; set; }

        [MaxLength(250)]
        public required string Message { get; set; }
    }
}
