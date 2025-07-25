using BLL.ModelsDto;

namespace BLL.Interfaces
{
    public interface ICommentService
    {
        Task<Result<CommentDto>> CreateAsync(CommentDto comment);

        Task<Result<IEnumerable<CommentDto>>> GetAsync(int articleId, int count = 0);

        Task<Result<CommentDto>> UpdateAsync(CommentDto dto);

        Task<Result<string>> DeleteAsync(Guid commentId, string userId, bool isAdmin = false);
        Task<Result<CommentDto>> GetByIdAsync(Guid id);
    }
}