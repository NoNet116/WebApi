using AutoMapper;
using BLL.Interfaces;
using BLL.Models;
using BLL.ModelsDto;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;

namespace BLL.Services
{
    public class UserService : IUserService
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> _userManager;
        private readonly IMapper _mapper;
        IRepository<User> _userRepository;
        private readonly IRoleService _roleService;


        public UserService(Microsoft.AspNetCore.Identity.UserManager<User> userManager, IMapper mapper, IRepository<User> userRepository, IRoleService roleService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _userRepository = userRepository;
            _roleService = roleService;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            var result = new List<UserDto>();
            foreach (var user in users)
            {
                var dto  = _mapper.Map<UserDto>(user);
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault();
                dto.Role = role;
                result.Add(dto);
            }
                return result;
        }

        public async Task<UserDto> GetUserByIdAsync(string id)
        {
            var userEntity = await _userManager.FindByIdAsync(id);
            return _mapper.Map<UserDto>(userEntity);
        }

        public async Task<Result<UserDto>> CreateUserAsync(UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);

            var createResult = await _userManager.CreateAsync(user, userDto.Password);

            if (!createResult.Succeeded)
                return Result<UserDto>.Fail(500, createResult.Errors.Select(e => $"{e.Code}. {e.Description}").ToArray());

            // Проверяем или создаём роль
            var roleResult = await _roleService.GetByNameAsync(DefaultRoleConfig.DefaultRoleName);
            if (roleResult.Data == null)
            {
                roleResult = await _roleService.Create(DefaultRoleConfig.DefaultRoleName);
            }

            if (roleResult.Data == null)
                return Result<UserDto>.Fail(500, string.Format("Не удалось создать или получить роль {0}.", DefaultRoleConfig.DefaultRoleName));

            // Добавляем пользователя в роль
            var addToRoleResult = await _userManager.AddToRoleAsync(user, roleResult.Data.Name);
            if (!addToRoleResult.Succeeded)
                return Result<UserDto>.Fail(500, addToRoleResult.Errors.Select(e => $"{e.Code}. {e.Description}").ToArray());

            var dto = _mapper.Map<UserDto>(user);
            return Result<UserDto>.Ok(201, dto);

        }



        public async Task<Result<bool>> DeleteUserAsync(string id)
        {
            var userEntity = await _userManager.FindByIdAsync(id);

            if (userEntity == null)
                return Result<bool>.Fail(404,"Пользователь с указанным ID не найден.");

            var result = await _userManager.DeleteAsync(userEntity);

            return result.Succeeded
                ? Result<bool>.Ok(204, true)
                : Result<bool>.Fail(500, result.Errors.Select(e => e.Code + ". " + e.Description).ToArray());
        }

        public async Task<Result<IEnumerable<UserDto>>> GetUsersAsync(string? searchuser)
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();
                if (!string.IsNullOrEmpty(searchuser))
                    users = users.Where(u => string.Join(" ", u.LastName, u.FirstName, u.FatherName, u.UserName)
                .Contains(searchuser, StringComparison.CurrentCultureIgnoreCase)).ToList();

                var dto = _mapper.Map<IEnumerable<UserDto>>(users);
                return Result<IEnumerable<UserDto>>.Ok(200, dto);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<UserDto>>.Fail(500, ex.Message);
            }
        }

        public async Task<Result<string>> EditUserRoleAsync(string userId, string newRole)
        {
            // Параллельно получаем пользователя и проверку роли
            var userTask = _userManager.FindByIdAsync(userId);
            var roleTask = _roleService.GetByNamesAsync(newRole);

            await Task.WhenAll(userTask, roleTask);

            var user = userTask.Result;
            var roleExists = roleTask.Result;

            if (user == null)
                return Result<string>.Fail(404, "Пользователь не найден");

            if (roleExists.DataIsNull)
                return Result<string>.Fail(404, $"Роль '{newRole}' не найдена");

            // Удаление текущих ролей пользователя
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                    return Result<string>.Fail(500, "Ошибка при удалении текущих ролей");
            }

            // Добавление новой роли
            var addResult = await _userManager.AddToRoleAsync(user, newRole);
            if (!addResult.Succeeded)
                return Result<string>.Fail(500, "Ошибка при добавлении новой роли");

            // Обновление security stamp
            await _userManager.UpdateSecurityStampAsync(user);

            return Result<string>.Ok(200, "Роль пользователя успешно обновлена");
        }

    }
}
