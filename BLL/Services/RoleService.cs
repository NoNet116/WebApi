using AutoMapper;
using BLL.Interfaces;
using BLL.Models;
using BLL.ModelsDto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly ILogger<RoleService> _logger;
        private const string NOTFOUNDBYID = "Роль не найдена по Id.";
        private const string NOTFOUNDBYNAME = "Роль не найдена по имени.";
        public RoleService(RoleManager<IdentityRole> roleManager, IMapper mapper, ILogger<RoleService> logger)
        {
            _roleManager = roleManager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<RoleDto>> Create(string name)
        {

            if (string.IsNullOrWhiteSpace(name))
            {
                var msg = "Имя роли не может быть пустым.";
                _logger.LogError(msg);
                return Result<RoleDto>.Fail(msg);
            }

            var roleExists = await _roleManager.RoleExistsAsync(name);
            if (roleExists)
            {
                var msg = "Роль с таким именем уже существует.";
                _logger.LogError(msg);
                return Result<RoleDto>.Fail(msg);
            }

            var identityRole = new IdentityRole(name);

            var result = await _roleManager.CreateAsync(identityRole);

            if (!result.Succeeded)
            {
                var msg = result.Errors.Select(e => e.Description).ToArray();
                _logger.LogError( string.Join("\n", msg));
                Result<RoleDto>.Fail(msg);
            }

            string message = $"Создана роль: {name}";
            _logger.LogInformation(message);
            return Result<RoleDto>.Ok(_mapper.Map<RoleDto>(identityRole));
            
        }


    
        public async Task<Result<bool>> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                _logger.LogError(NOTFOUNDBYID);
                return Result<bool>.Fail(NOTFOUNDBYID);
            }
            
            if (role.Name == RoleType.User.ToString() || role.Name == RoleType.Administrator.ToString())
            {
                return Result<bool>.Fail("Нельзя удалять роль по умолчанию");
            }

            var result = await _roleManager.DeleteAsync(role);

            if (!result.Succeeded)
            {
                var msg = result.Errors.Select(e => e.Description).ToArray();
                _logger.LogError(string.Join("\n", msg));
                Result<RoleDto>.Fail(msg);
            }
            string message = $"Удалена роль: {role.Name}";
            _logger.LogInformation(message);
            return Result<bool>.Ok(true);
        }

        public async Task<IEnumerable<RoleDto>> GetAllAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return _mapper.Map<IEnumerable<RoleDto>>(roles);

        }

        public async Task<Result<RoleDto>> GetByNameAsync(string name)
        {
            return await GetRoleAsync(() => _roleManager.FindByNameAsync(name), NOTFOUNDBYNAME);
        }

        public async Task<Result<RoleDto>> GetByIdAsync(string id)
        {
            return await GetRoleAsync(() => _roleManager.FindByIdAsync(id), NOTFOUNDBYID);
        }

        private async Task<Result<RoleDto>> GetRoleAsync(Func<Task<IdentityRole?>> findFunc, string notFoundBy)
        {
            var role = await findFunc();
            if (role == null)
            {
                _logger.LogError(notFoundBy);
                Result<RoleDto>.Fail(notFoundBy);
            }
            return Result<RoleDto>.Ok(_mapper.Map<RoleDto>(role));
        }

        public async Task<Result<RoleDto>> UpdateAsync(RoleDto roleDto)
        {
            if (string.IsNullOrEmpty(roleDto.Id))
            {
                var msg = "Id не может быть пустым.";
                _logger.LogError(msg);
                return Result<RoleDto>.Fail(msg);
            }

            var role = await _roleManager.FindByIdAsync(roleDto.Id);
            if (role == null)
            {
                _logger.LogError(NOTFOUNDBYID);
                return Result<RoleDto>.Fail(NOTFOUNDBYID); 
            }

            if (string.IsNullOrWhiteSpace(roleDto.Name))
            {
                var msg = "Имя роли не может быть пустым.";
                _logger.LogError(msg);
                return Result<RoleDto>.Fail(msg); ;
            }

            var oldname = role.Name;
            role.Name = roleDto.Name;
            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
                return Result<RoleDto>.Fail(result.Errors.Select(e => e.Description).ToArray());
            

            string message = $"Изменена роль: {oldname} => {roleDto.Name}";
            _logger.LogInformation(message);

            return Result<RoleDto>.Ok(_mapper.Map<RoleDto>(role));
        }

        public async Task<Result<IEnumerable<RoleDto>>> GetByNamesAsync(string name)
        {
            // Проверяем, что строка не пустая
            if (string.IsNullOrWhiteSpace(name))
            {
                return Result<IEnumerable<RoleDto>>.Fail("Имя роли не может быть пустым.");
            }

            var allroles = await _roleManager.Roles.ToListAsync();

            var roles = allroles
                .Where(r => r.Name.Contains(name, StringComparison.CurrentCultureIgnoreCase));


            // Если ничего не найдено
            if (!roles.Any())
            {
                return Result<IEnumerable<RoleDto>>.Fail("Роли с таким именем не найдены.");
            }

            var dto = _mapper.Map<IEnumerable<RoleDto>>(roles);

            return Result<IEnumerable<RoleDto>>.Ok(dto);
        }

    }
}
