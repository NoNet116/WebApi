using BLL.Interfaces;
using BLL.ModelsDto;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.ViewModels.Comments;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost]
        public IActionResult TestResponse ([FromBody] GetCommentViewModel model)
        {
            var cmnt = new Comment()
            {
                Id = "1",
                Message = "Тестовый ответ",
                Author = "Иван Иванов",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            var vm = new CommentViewModel() {ArticleId = model.ArticleId, Comments = [cmnt]  };
            return StatusCode(200, vm);
        }
       
        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] CreateCommentViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // Лучше вернуть ModelState, а не модель

            var userId = User?.Identity?.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User is not authenticated.");

            var dto = new CommentDto
            {
                ArticleId = model.ArticleId,
                Message = model.Message,
                AuthorId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _commentService.CreateAsync(dto);

            if (!result.Success)
                return StatusCode(result.StatusCode, string.Join("\r\n", result.Errors));

            return StatusCode(result.StatusCode, result.Data);
            return Ok();
        }

    }
}
