
using AutoMapper;
using BLL.Interfaces;
using BLL.ModelsDto;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class ArticleService : IArticleService, IBaseService<Article, ArticleDto>
    {
        private readonly IRepository<Article> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<ArticleService> _logger;
        private readonly IUserService _userService;

        public ArticleService(IRepository<Article> repository, IMapper mapper, ILogger<ArticleService> logger, IUserService userService)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _userService = userService;
        }

        private Result<ArticleDto>? IsVaild(ArticleDto dto)
        {
            if (string.IsNullOrEmpty(dto.Content))
                return Result<ArticleDto>.Fail(400, "Не задано содержание");

            if (string.IsNullOrEmpty(dto.Title))
                return Result<ArticleDto>.Fail(400, "Не задан заголовок");

            return null;
        }
        public async  Task<Result<ArticleDto>> CreateAsync(ArticleDto dto)
        {
            var dtovalid = IsVaild(dto);
            if (dtovalid != null)
                return dtovalid;


            try
            {

                var userDto = await _userService.GetUserByIdAsync(dto.AuthorId);

                if (userDto == null)
                    return Result<ArticleDto>.Fail(404, "User not found.");

                var entity = _mapper.Map<Article>(dto);

                await _repository.AddAsync(entity);

                return Result<ArticleDto>.Ok(201, null);
            }
            catch (Exception ex)
            {
                var detailedMessage = ex.InnerException?.Message ?? ex.Message;

                return Result<ArticleDto>.Fail(500, detailedMessage);
            }
        }
    }
}
