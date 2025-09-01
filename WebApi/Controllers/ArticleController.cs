using AutoMapper;
using BLL;
using BLL.Interfaces;
using BLL.ModelsDto;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.ViewModels.Articles;

namespace WebApi.Controllers
{
    [ApiController, Authorize]
    [Route("api/[controller]")]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleService _articleService;
        private readonly ILogger<ArticleController> _logger;
        private readonly IMapper _mapper;

        public ArticleController(IArticleService articleService, IMapper mapper, ILogger<ArticleController> logger)
        {
            _articleService = articleService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateArticleViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var dto = _mapper.Map<ArticleDto>(model);
            dto.AuthorId = User.Identity.GetUserId();
            var result = await _articleService.CreateAsync(dto);

            if (!result.Success)
                return StatusCode(result.StatusCode, string.Join("\n \r", result.Errors));

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("Create2")]
        public async Task<IActionResult> Create2([FromBody] CreateArticleViewModel model)
        {
            var dto = _mapper.Map<ArticleDto>(model);
            dto.AuthorId = User.Identity.GetUserId();
            var result = await _articleService.CreateAsync2(dto);
            if (!result.Success)
                return StatusCode(result.StatusCode, string.Join("\n \r", result.Errors));

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        public async Task<IActionResult> FindByTitle(string? title)
        {
            var res = await _articleService.FindByTitleAsync(title);
            return StatusCode(res.StatusCode, res?.Data);
        }

        [HttpGet("{startIndex}/{count}")]
        public async Task<IActionResult> Get(int startIndex = 0, int count = 10)
        {
            (int startIndex, int count) item;
            item.startIndex = startIndex;
            item.count = count;
            var res = await _articleService.GetLatestArticlesAsync(item);
            return StatusCode(res.StatusCode, res.Data);
        }

        [HttpGet("author/{authorId}")]
        public async Task<IActionResult> GetByAuthor(string authorId)
        {
            var result = await _articleService.GetByAuthorIdAsync(authorId);

            if(!result.Success)
                return StatusCode(result.StatusCode, string.Join("\r\n", result.Errors));

            return StatusCode(result.StatusCode, result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> FindById(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID");

            try
            {
                var result = await _articleService.FindByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding article by id {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}