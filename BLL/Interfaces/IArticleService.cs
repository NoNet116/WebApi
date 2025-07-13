
using BLL.ModelsDto;

namespace BLL.Interfaces
{
    public interface IArticleService
    {
        Task<Result<ArticleDto>> CreateAsync(ArticleDto dto);
        Task<Result<IEnumerable<ArticleDto>>> FindByTitleAsync(string? title = null);

        Task<Result<IEnumerable<ArticleDto>>> GetLatestArticlesAsync(int count = 10);
    }
}
