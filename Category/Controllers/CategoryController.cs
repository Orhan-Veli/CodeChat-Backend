
using System;
using System.Web;
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
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _categoryService.GetAllAsync();          
            return StatusCode((int)result.Response,new { result.Message ,result.Data});
        }

        [HttpPost]
        public async Task<IActionResult> BulkCreateAsync([FromBody] List<CategoryDto> categories)
        {
            var result = await _categoryService.BulkCreateAsync(categories);            
            return StatusCode((int)result.Response,new { result.Message ,result.Data});
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync(CategoryDto category)
        {   
            var result = await _categoryService.CreateAsync(category);
            return StatusCode((int)result.Response,new { result.Message ,result.Data});
        }

        [HttpDelete]
        public async Task<IActionResult> BulkDeleteAsync()
        {
            var result = await _categoryService.BulkDeleteAsync();
            return StatusCode((int)result.Response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var result = await _categoryService.DeleteAsync(id);
            return StatusCode((int)result.Response);
        }
    }
}