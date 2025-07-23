
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
        private readonly IRepository<Article> _articleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ArticleService> _logger;
        private readonly IUserService _userService;
        private readonly ITagService _tagService;
        private readonly IArticleTagService _articleTagService;

        public ArticleService(IRepository<Article> repository, IMapper mapper, ILogger<ArticleService> logger, IUserService userService, ITagService tagService, IArticleTagService IArticleTagService)
        {
            _articleRepository = repository;
            _mapper = mapper;
            _logger = logger;
            _userService = userService;
            _tagService = tagService;
            _articleTagService = IArticleTagService;

        }

        #region Create
       

        public async Task<Result<ArticleDto>> CreateAsync(ArticleDto dto)
        {
            // Валидация DTO
            if (string.IsNullOrWhiteSpace(dto.Title))
                return Result<ArticleDto>.Fail(400, "Заголовок статьи обязателен");

            if (string.IsNullOrWhiteSpace(dto.Content))
                return Result<ArticleDto>.Fail(400, "Содержание статьи обязательно");

            if (string.IsNullOrWhiteSpace(dto.AuthorId))
                return Result<ArticleDto>.Fail(400, "Не указан автор статьи");

            try
            {
                // Проверка существования пользователя
                var userExists = await _userService.GetUserByIdAsync(dto.AuthorId);
                if (userExists == null)
                    return Result<ArticleDto>.Fail(404, "Пользователь не найден");

                // Получение тегов (оптимизированная версия)
                var tags = new List<Tag>();
                if (dto.Tags != null && dto.Tags.Any())
                {
                    var existingTags = await _tagService.GetExistingTagsAsync(dto.Tags);
                    var notFoundTags = dto.Tags.Except(existingTags.Select(t => t.Name));

                    if (notFoundTags.Any())
                        return Result<ArticleDto>.Fail(404, $"Теги не найдены: {string.Join(", ", notFoundTags)}");

                    tags.AddRange(existingTags);
                }

                // Маппинг и сохранение
                var article = new Article
                {
                    Title = dto.Title,
                    Content = dto.Content,
                    DescriptionEntity = dto.DescriptionDto ?? string.Empty,
                    AuthorId = dto.AuthorId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ArticleTags = tags.Select(t => new ArticleTags { TagId = t.Id }).ToList()
                };

                await _articleRepository.AddAsync(article);

                // Маппинг результата
                var resultDto = new ArticleDto
                {
                    Id = article.Id,
                    Title = article.Title,
                    Content = article.Content,
                    DescriptionDto = article.DescriptionEntity,
                    CreatedAt = article.CreatedAt,
                    UpdatedAt = article.UpdatedAt,
                    AuthorId = article.AuthorId,
                    TagsCount = tags.Count,
                    //Tags = tags.Select(t => new TagDto { Id = t.Id, Name = t.Name }).ToList()
                    Tags = tags.Select(t => t.Name).ToList()
                };

                return Result<ArticleDto>.Ok(201, resultDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании статьи. DTO: {@Dto}", dto);
                return Result<ArticleDto>.Fail(500, "Не удалось создать статью");
            }
        }

        public async Task<Result<ArticleDto>> CreateAsync2(ArticleDto dto)
        {
            #region Валидация DTO
            if (string.IsNullOrWhiteSpace(dto.Title))
                return Result<ArticleDto>.Fail(400, "Заголовок статьи обязателен");

            if (string.IsNullOrWhiteSpace(dto.Content))
                return Result<ArticleDto>.Fail(400, "Содержание статьи обязательно");

            if (string.IsNullOrWhiteSpace(dto.AuthorId))
                return Result<ArticleDto>.Fail(400, "Не указан автор статьи");
            #endregion

            try
            {
                #region Проверка автора
                var authorExists = await _userService.GetUserByIdAsync(dto.AuthorId) != null;
                if (!authorExists)
                    return Result<ArticleDto>.Fail(404, "Автор не найден");
                #endregion

                #region Получение тегов
                var tags = new List<Tag>();
                if (dto.Tags != null && dto.Tags.Any())
                {
                    var existingTags = await _tagService.GetExistingTagsAsync(dto.Tags);
                    var notFoundTags = dto.Tags.Except(existingTags.Select(t => t.Name));

                    if (notFoundTags.Any())
                        return Result<ArticleDto>.Fail(404, $"Теги не найдены: {string.Join(", ", notFoundTags)}");

                    tags.AddRange(existingTags);
                }
                #endregion

                // Создание статьи
                var article = new Article
                {
                    Title = dto.Title,
                    Content = dto.Content,
                    DescriptionEntity = dto.DescriptionDto ?? string.Empty,
                    AuthorId = dto.AuthorId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _articleRepository.AddAsync(article);

                #region Создание связей с тегами
                if (tags.Any())
                {
                    var tagTasks = tags.Select(tag =>
                        _articleTagService.CreateAsync(new ArticleTagCreateDto
                        {
                            TagId = tag.Id,
                            ArticleId = article.Id // Исправлено: используем ID созданной статьи
                        }));

                    await Task.WhenAll(tagTasks);
                }
                #endregion

                // Маппинг результата
                var resultDto = new ArticleDto
                {
                    Id = article.Id,
                    Title = article.Title,
                    Content = article.Content,
                    DescriptionDto = article.DescriptionEntity,
                    CreatedAt = article.CreatedAt,
                    UpdatedAt = article.UpdatedAt,
                    AuthorId = article.AuthorId,
                    TagsCount = tags.Count,
                    Tags = tags.Select(t => t.Name ).ToList()
                };

                return Result<ArticleDto>.Ok(201, resultDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании статьи");
                return Result<ArticleDto>.Fail(500, "Не удалось создать статью");
            }
        }

        #endregion

        #region Find
        public async Task<ArticleDto> FindByIdAsync (int id)
        {
            var query = _articleRepository.GetQueryable()
        .Include(a => a.Author)
        .Where(a => a.Id == id)
        .Select(a => new ArticleDto
        {
            Id = a.Id,
            Title = a.Title,
            Content = a.Content,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt,
            AuthorId = a.AuthorId,
            AuthorName = a.Author.UserName
            // Добавь при необходимости другие свойства
        });

            return await query.FirstOrDefaultAsync();
        }
        public async Task<Result<IEnumerable<ArticleDto>>> FindByTitleAsync(string? title = null)
        {

            var query = _articleRepository.GetQueryable()
            .Include(a => a.Author)
            .Select(a => new ArticleDto
            {
                
                Id = a.Id,
                Title = a.Title,
                Content = a.Content,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt,
                AuthorId = a.AuthorId,
                AuthorName = a.Author.UserName
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


        public async Task<Result<IEnumerable<ArticleDto>>> GetLatestArticlesAsync(int count = 10)
        {
            try
            {
                var articles = await _articleRepository.GetQueryable()
                    .Include(a => a.Author)              // Загружаем автора
                //    .Include(a => a.ArticleTags)         // Загружаем связи с тегами
                //    .ThenInclude(at => at.Tag)       // Загружаем сами теги
                  //  .Include(a => a.Comments)           // Загружаем комментарии
                    .OrderByDescending(a => a.CreatedAt) // Сортируем по дате создания (новые сначала)
                    .Take(count)                        // Берем только указанное количество
                    .AsNoTracking()                     // Для read-only операций
                    .ToListAsync();

                var result = articles.Select(a => new ArticleDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Content = a.Content,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt,
                    AuthorId = a.AuthorId,
                    AuthorName = a.Author.UserName,
                    TagsCount = a.ArticleTags.Count,
                    CommentsCount = a.Comments.Count,
                    Tags = (List<string>)a.ArticleTags.Select(at => at.Tag.Name)
                });

                return Result<IEnumerable<ArticleDto>>.Ok(200, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении последних статей");
                return Result<IEnumerable<ArticleDto>>.Fail(500, "Не удалось загрузить статьи");
            }
        }
    }
}
