using AutoMapper;
using BLL.Interfaces;
using BLL.ModelsDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase 
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet("All")]
        [ProducesResponseType(typeof(IEnumerable<UserViewModel>), StatusCodes.Status200OK)]
       
        public async Task<IActionResult> GetAll()
        {
            var usersDto = await _userService.GetAllUsersAsync();
            if (usersDto == null)
                return NotFound();
            return Ok(_mapper.Map<IEnumerable<UserViewModel>>(usersDto));
        }

        
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(_mapper.Map<UserViewModel>(user));
        }
       
        [AllowAnonymous, HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] RegisterUserModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userDto = _mapper.Map<UserDto>(model);
            var result = await _userService.CreateUserAsync(userDto);

            if (!result.Success)
                return BadRequest(new { Errors = result.Errors });

            // Получаем созданного пользователя (например, с ID/email)
            var createdUser = result.Data!;

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdUser.Id }, 
                createdUser
            );
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Id не может быть пустым.");

            var result = await _userService.DeleteUserAsync(id);

            return result.Success
                ? Ok("Пользователь успешно удалён.")
                : BadRequest(result.Errors);
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(UserViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Не удалось определить пользователя.");

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound();

            return Ok(_mapper.Map<UserViewModel>(user));
        }



    }


}
