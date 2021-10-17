using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Category.Dal.Model;
using Category.Utilities.Concrete;
using Category.Dal.Repositories.Abstract;
using Category.Dal.Repositories.Concrete;
using System.Threading.Tasks;
using Category.Utilities.Abstract;
using Category.Business.Abstract;
using Category.Dto;
using Mapster;
using Serilog;
using Serilog.Events;
namespace Category.Business.Concrete
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<IResult<bool>> BulkCreateAsync(List<CategoryDto> categories)
        {
            if(categories.Any(x=>x.Image == null && x.Name == null))
            {
                return new Result<bool>(false,"Model is not usefull",HttpStatusCode.BadRequest);
            }
            List<CategoryModel> categoryModel = new List<CategoryModel>();
            foreach (var item in categories)
            {
                categoryModel.Add(new CategoryModel
                {
                    Id = Guid.NewGuid(),
                    Image = item.Image,
                    Name = item.Name,
                    CreatedOn = DateTime.Now.ToString()
                });
            }
            var result = await _categoryRepository.BulkCreateAsync(categoryModel);
            Log.Logger.Information("CategoryCreated" + DateTime.Now);
            return new Result<bool>(result,HttpStatusCode.Created);
        }

        public async Task<IResult<List<CategoryModel>>> GetAllAsync()
        {
            var result = await _categoryRepository.GetAllAsync();
            if (!result.Any())
            {
                return new Result<List<CategoryModel>>(true,new List<CategoryModel>(),HttpStatusCode.Ok);
            }
            return new Result<List<CategoryModel>>(true, result,HttpStatusCode.Ok);
        }

        public async Task<IResult<bool>> BulkDeleteAsync()
        {
            Log.Logger.Information("CategoryDeleted" + DateTime.Now);
            var result = await _categoryRepository.BulkDeleteAsync();
            return new Result<bool>(result,HttpStatusCode.NoContent);
        }
        public async Task<IResult<CategoryDto>> CreateAsync(CategoryDto category)
        {
            if(category == null || category.Image == null ||category.Name == null)
            {
                return new Result<CategoryDto>(false,"Category model is not valid",HttpStatusCode.BadRequest);
            }
            var categoryModel = category.Adapt<CategoryModel>();
            var result = await _categoryRepository.CreateAsync(categoryModel);
            return new Result<CategoryDto>(true,HttpStatusCode.Created);
        }

        public async Task<IResult<bool>> DeleteAsync(Guid id)
        {
            if(id == Guid.Empty)
            {
                return new Result<bool>(false,HttpStatusCode.BadRequest);
            }
            var result = await _categoryRepository.DeleteAsync(id);
            return new Result<bool>(true,HttpStatusCode.NoContent);
        }
    }
}