
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Category.Dto;
using Category.Business.Abstract;
using Category.Business.Concrete;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Microsoft.AspNetCore.Cors;

namespace Category.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("_myAllowSpecificOrigins")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllModels()
        {
            var result = await _categoryService.GetAll();
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] List<CategoryDto> category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _categoryService.Create(category);
            if (!result.Success)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpDelete]
        public async Task<ActionResult> Delete()
        {
            var result = await _categoryService.Delete();
            if (!result.Success)
            {
                return BadRequest();
            }
            return NoContent();
        }
    }
}