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
        Task<IResult<bool>> Create(List<CategoryDto> category);
        Task<IResult<List<CategoryModel>>> GetAll();
        Task<IResult<bool>> Delete();
    }
}