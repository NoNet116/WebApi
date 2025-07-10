using AutoMapper;
using BLL;
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
        public async Task<IActionResult> Create([FromBody] RegisterTagModel model)
        {
            var dto = _mapper.Map<TagDto>(model);
            var result = await _tagService.CreateAsync(dto, User);
            return StatusCode(result.StatusCode, result);
        }
        #endregion

        #region Find Tag
        [HttpGet("by-name/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> FindByName(string? name)
        {
            var result = await _tagService.FindByNameAsync(name);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("by-id/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> FindById(Guid id)
        {
            var result = await _tagService.FindByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        #endregion

        #region Update Tag
        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] UpdateViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var dto = _mapper.Map<TagDto>(model);
            var result = await _tagService.UpdateAsync(dto);

            return StatusCode(result.StatusCode, result);
      }
        #endregion

        #region Delete Tag
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete (Guid id)
        {
            var result = await _tagService.DeleteAsync(id);

            return StatusCode(result.StatusCode, result);
        }
        #endregion
    }
}
