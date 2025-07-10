using AutoMapper;
using BLL.ModelsDto;
using WebApi.ViewModels;
using WebApi.ViewModels.Tags;

namespace WebApi.Mapper
{
    public class PLLMappingProfile : Profile
    {
        public PLLMappingProfile() {
            CreateMap<UserViewModel, UserDto>().ReverseMap();
            CreateMap<RegisterUserModel, UserDto>().ReverseMap();

            RoleMap();
            TagMap();
        }

        void RoleMap()
        {
            CreateMap<RoleViewModel, RoleDto>().ReverseMap();

            CreateMap<RegisterRoleModel, RoleDto>();
            
        }

        void TagMap()
        {
            CreateMap<TagViewModel, TagDto>().ReverseMap();
            CreateMap<RegisterTagModel, TagDto>().ReverseMap();
        }
    }
}
