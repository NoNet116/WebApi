using AutoMapper;
using BLL.Interfaces;
using BLL.ModelsDto;
using DAL.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.ViewModels.Tags;

namespace WebApi.Controllers
{
    
    [ApiController, Authorize]
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;
        private readonly IMapper _mapper;

        public TagController(ITagService tagService, IMapper mapper) {
            _tagService = tagService;
            _mapper = mapper;
        }
        #region Create Tag

        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] RegisterTagModel model)
        {
            var dto = _mapper.Map<TagDto>(model);
            var t = await _tagService.CreateAsync(dto, User);
            return Ok(t.Success + " " + t?.Errors);
        }
        #endregion

        #region GetAll Tag
        [HttpPost("All")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll(string name)
        {
            return Ok();
        }
        #endregion
    }
}
