
using AutoMapper;
using BLL.Interfaces;
using BLL.ModelsDto;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        #region Create
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
        #endregion

        #region Find
        public async Task<Result<IEnumerable<ArticleDto>>> FindByTitleAsync(string? title = null)
        {

            var query = _repository.GetQueryable()
            .Include(a => a.Author)
            .Select(a => new ArticleDto
            {
                Id = a.Id,
                Title = a.Title,
                Content = a.Content,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt,
                AuthorId = a.AuthorId,
                AuthorName = a.Author.UserName,
                TagsCount = a.Tags.Count,
                CommentsCount = a.Comments.Count,
                // Остальные свойства
            });

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(a => a.Title.Contains(title));
            }

            var dto = await query.ToListAsync();

            return Result<IEnumerable<ArticleDto>>.Ok(200, dto);

            /* Пояснение
             1. _repository.GetQueryable()
             Этот метод возвращает IQueryable<T>, который представляет собой запрос к базе данных, но ещё не выполненный.
             На этом этапе мы только начинаем строить SQL-запрос, но ничего не загружаем.
             
             2. .Include(a => a.Author)
             Добавляет JOIN к таблице Author, чтобы загрузить данные об авторе статьи.
             Без этого EF Core загрузил бы только AuthorId, но не сам объект Author.
             
             3. .Select(a => new ArticleDto { ... })
             Это проекция – преобразование данных из сущности Article в ArticleDto прямо в SQL-запросе.
             Вместо загрузки всех полей Article, включая связанные коллекции (Tags, Comments), мы сразу выбираем только нужные данные.
             
             4. Заполнение ArticleDto
             Каждое свойство ArticleDto заполняется данными из Article и связанных таблиц:
             
             Id, Title, Content – берутся напрямую из Article.
             
             AuthorId – из Article.AuthorId.
             
             AuthorName – из Article.Author.UserName (т.к. мы сделали .Include(a => a.Author)).
             
             TagsCount – вычисляется как a.Tags.Count (EF Core преобразует это в COUNT в SQL).
             
             CommentsCount – аналогично, a.Comments.Count.
             
             ⚡ Важно: Почему это эффективно?
             Нет лишних данных – мы не загружаем все Tags и Comments, только их количество.
             
             Всё считается на стороне БД – Count выполняется в SQL, а не в памяти.
             
             Меньше трафика – клиент получает только готовый ArticleDto, а не все сущности.
             */

        }
        #endregion
    }
}
