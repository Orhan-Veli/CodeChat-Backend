using Category.Dal.Model;
using Category.Utilities.Abstract;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Category.Dto;
namespace Category.Business.Abstract
{
    public interface ICategoryService
    {
        Task<IResult<bool>> BulkCreateAsync(List<CategoryDto> categories);
        Task<IResult<List<CategoryModel>>> GetAllAsync();
        Task<IResult<bool>> BulkDeleteAsync();
        Task<IResult<CategoryDto>> CreateAsync(CategoryDto category);
        Task<IResult<bool>> DeleteAsync(Guid id);
    }
}