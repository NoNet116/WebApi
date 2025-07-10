using AutoMapper;
using BLL.Interfaces;
using BLL.ModelsDto;
using DAL.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
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

        #region Find Tag
        [HttpGet("by-name/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> FindByName(string? name)
        {
            var tags = await _tagService.FindByNameAsync(name);
            if (!tags.Success)
                return BadRequest(tags);

            if (tags.DataIsNull)
                return NoContent();

            return Ok(tags.Data);
        }

        [HttpGet("by-id/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> FindById(Guid id)
        {
            var tags = await _tagService.FindByIdAsync(id);
            if (!tags.Success)
                return BadRequest(tags);

            if (tags.DataIsNull)
                return NoContent();

            return Ok(tags.Data);
        }

        #endregion

        #region Update Tag
        [HttpGet("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Update([FromBody] UpdateViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok();
        }
        #endregion
    }
}
