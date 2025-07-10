using AutoMapper;
using BLL.Interfaces;
using DAL.Interfaces;

namespace BLL.Services
{
    public class Service<T>:IService<T>where T : class
    {
        private readonly IMapper _mapper;
        private readonly IRepository<T> _repository;
       
        public Service(T value, IMapper mapper, IRepository<T> repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var values = await _repository.GetAllAsync();

            return _mapper.Map<IEnumerable<T>>(values);
        }

        public async Task<Result<bool>> CreateAsync(T value)
        {
           
            try
            {
                var entity = _mapper.Map<T>(value);
                await _repository.AddAsync(entity);
                
                return Result<bool>.Ok(201, true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail(500, ex.Message);
            }
        }

    }
}
