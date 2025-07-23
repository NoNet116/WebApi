using BLL.ModelsDto;

namespace BLL.Interfaces
{
    public interface ICommentService
    {
        Task<Result<CommentDto>> CreateAsync(CommentDto comment);
    }
}
