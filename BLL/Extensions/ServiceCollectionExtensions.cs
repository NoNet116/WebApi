using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BLL.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Создание роли по умолчанию
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityRoles(this IServiceCollection services)
        {
            // Временно создаем сервис-провайдер
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                string[] roles = new[] { "User", "Administrator" };

                foreach (var role in roles)
                {
                    var roleExists = roleManager.RoleExistsAsync(role).GetAwaiter().GetResult();
                    if (!roleExists)
                    {
                        roleManager.CreateAsync(new IdentityRole(role)).GetAwaiter().GetResult();
                    }
                }
            }

            return services;
        }
    }
}
