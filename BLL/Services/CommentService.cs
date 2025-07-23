using AutoMapper;
using BLL.Interfaces;
using BLL.ModelsDto;
using DAL.Entities;
using DAL.Interfaces;
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
    }
}
