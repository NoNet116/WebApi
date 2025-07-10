using BLL.ModelsDto;
using System.Security.Claims;

namespace BLL.Interfaces
{
    public interface ITagService
    {
        Task<Result<bool>> CreateAsync(TagDto tagDto, ClaimsPrincipal user);
        Task<Result<IEnumerable<TagDto>>> FindByNameAsync(string? name = null);
    }
}
