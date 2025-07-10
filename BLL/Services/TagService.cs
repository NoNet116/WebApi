using AutoMapper;
using BLL.Interfaces;
using BLL.ModelsDto;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace BLL.Services
{
    public class TagService : ITagService
    {
        private readonly IRepository<Tag> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<TagService> _logger;

        public TagService(IRepository<Tag> repository, IMapper mapper, ILogger<TagService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<bool>> CreateAsync(TagDto tagDto, ClaimsPrincipal user)
        {
            try
            {
                if (string.IsNullOrEmpty(tagDto.Name))
                    throw new ArgumentNullException("No name is set for the tag");

                if(user == null)
                    throw new ArgumentNullException("The ClaimsPrincipal user must not be null");
                            
                var authorid = user.Identity.GetUserId();

                var tagEntity = _mapper.Map<Tag>(tagDto);
                tagEntity.CreatedByUserId = authorid;
                await _repository.AddAsync(tagEntity);
                return Result<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail(ex.Message);
            }
        }
    }
}
