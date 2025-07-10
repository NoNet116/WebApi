
using BLL.ModelsDto;

namespace BLL.Interfaces
{
    public interface IArticleService
    {
        Task<Result<ArticleDto>> CreateAsync(ArticleDto dto);
    }
}
