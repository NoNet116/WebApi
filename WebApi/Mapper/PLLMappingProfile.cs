using AutoMapper;
using BLL.ModelsDto;
using WebApi.ViewModels;

namespace WebApi.Mapper
{
    public class PLLMappingProfile : Profile
    {
        public PLLMappingProfile() {
            CreateMap<UserViewModel, UserDto>().ReverseMap();
            CreateMap<RegisterUserModel, UserDto>().ReverseMap();

            RoleMap();
        }

        void RoleMap()
        {
            CreateMap<RoleViewModel, RoleDto>().ReverseMap();

            CreateMap<RegisterRoleModel, RoleDto>();
            
        }
    }
}
