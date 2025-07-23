using AutoMapper;
using BLL.Interfaces;
using BLL.ModelsDto;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class CommentService : ICommentService
    {
        private readonly IRepository<Comment> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<CommentService> _logger;
        private readonly IArticleService _articleService;
        private readonly IUserService _userService;

        public CommentService(IRepository<Comment> repository,
            IMapper mapper,
            ILogger<CommentService> logger,
            IArticleService articleService,
            IUserService userService) 
        { 
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _articleService = articleService;
            _userService = userService;
        }

        public async Task<Result<CommentDto>> CreateAsync(CommentDto comment)
        {
            if(string.IsNullOrEmpty(comment.Message))
                return Result<CommentDto>.Fail(400, "Сообщение не должно быть пустым.");

            if (string.IsNullOrEmpty(comment.AuthorId))
                return Result<CommentDto>.Fail(400, "Не указан Id автора.");
            var author = await _userService.GetUserByIdAsync(comment.AuthorId);

            if(author == null)
                return Result<CommentDto>.Fail(401,"User not found");

            var article = await _articleService.FindByIdAsync(comment.ArticleId);
            if (article == null)
                return Result<CommentDto>.Fail(401, "Article not found");

            var entity = _mapper.Map<Comment>(comment);
            entity.AuthorId = comment.AuthorId;
            entity.ArticleId = comment.ArticleId;

            await _repository.AddAsync(entity);

            return Result<CommentDto>.Ok(201, comment);

        }

        public async Task<Result<IEnumerable<CommentDto>>> GetAsync(int articleId, int count = 0)
        {
            if (articleId <= 0)
                return Result<IEnumerable<CommentDto>>.Fail(400, "Некорректный ID статьи");

            IQueryable<Comment> query = _repository.GetQueryable()
                .Where(c => c.ArticleId == articleId)
                .OrderByDescending(c => c.CreatedAt);

            if (count > 0)
                query = query.Take(count);

            var comments = await query
                .Select(c => new CommentDto
                {
                    Id = c.Id.ToString(),
                    Message = c.Message,
                    Author = c.Author.UserName,
                    AuthorId = c.AuthorId,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    ArticleId = c.ArticleId
                })
                .ToListAsync();

            return Result<IEnumerable<CommentDto>>.Ok(200, comments);
        }
    }
}
