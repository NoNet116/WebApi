using AutoMapper;
using BLL.ModelsDto;
using DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace BLL.Mapper
{
    public class BLLMappingProfile :Profile
    {
        public BLLMappingProfile()
        {
            // From Entity to DTO
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.ProfileImage, opt => opt.MapFrom(src => src.Image));

            // From DTO to Entity
            CreateMap<UserDto, User>()
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.ProfileImage))
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // важно, иначе EF будет ругаться

            RoleMap();
        }

        void RoleMap()
        {
            CreateMap<IdentityRole, RoleDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            CreateMap<RoleDto, IdentityRole>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
        }
    }
}
