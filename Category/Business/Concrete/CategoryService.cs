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
        private readonly IRepository _categoryRepository;
        public CategoryService(IRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<IResult<bool>> Create(List<CategoryDto> category)
        {
            List<CategoryModel> categoryModel = new List<CategoryModel>();
            foreach (var item in category)
            {
                categoryModel.Add(new CategoryModel
                {
                    Id = Guid.NewGuid(),
                    Image = item.Image,
                    Name = item.Name,
                    CreatedOn = DateTime.Now.ToString()
                });
            }
            var result = await _categoryRepository.Create(categoryModel);
            Log.Logger.Information("CategoryCreated" + DateTime.Now);
            return new Result<bool>(result);
        }

        public async Task<IResult<List<CategoryModel>>> GetAll()
        {
            var result = await _categoryRepository.GetAll();
            if (!result.Any())
            {
                return new Result<List<CategoryModel>>(true,new List<CategoryModel>());
            }
            return new Result<List<CategoryModel>>(true, result);
        }

        public async Task<IResult<bool>> Delete()
        {
            Log.Logger.Information("CategoryDeleted" + DateTime.Now);
            var result = await _categoryRepository.Delete();
            return new Result<bool>(result);
        }
    }
}