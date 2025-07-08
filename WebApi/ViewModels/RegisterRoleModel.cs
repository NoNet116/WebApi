using System.ComponentModel.DataAnnotations;

namespace WebApi.ViewModels
{
    public class RegisterRoleModel
    {
        [Required, MaxLength(50)]
        public required string Name { get; set; }
    }
}
