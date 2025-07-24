using System.ComponentModel.DataAnnotations;

namespace WebApi.ViewModels.Comments
{
    public class EditCommentViewModel
    {
        [Required]
        public required Guid CommentId { get; set; }

        [Required]
        public required string Message { get; set; }
    }
}
