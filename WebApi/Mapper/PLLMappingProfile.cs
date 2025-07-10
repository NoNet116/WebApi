using AutoMapper;
using BLL.ModelsDto;
using WebApi.ViewModels;
using WebApi.ViewModels.Articles;
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
            ArticleMap();
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
            CreateMap<UpdateViewModel, TagDto>().ReverseMap();
        }

        void ArticleMap()
        {
            CreateMap<CreateArticleViewModel, ArticleDto>().ReverseMap();
        }
    }
}
