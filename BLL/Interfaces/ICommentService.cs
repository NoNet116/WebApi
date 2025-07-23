using BLL.ModelsDto;

namespace BLL.Interfaces
{
    public interface ICommentService
    {
        Task<Result<CommentDto>> CreateAsync(CommentDto comment);
        Task<Result<IEnumerable<CommentDto>>> GetAsync(int articleId, int count = 0);
    }
}
