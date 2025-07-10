
using System.Linq.Expressions;


namespace BLL.Interfaces
{
    public interface IService<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<Result<bool>> CreateAsync(T value);
    }
}
