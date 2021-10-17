using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Category.Dal.Model;
namespace Category.Dal.Repositories.Abstract
{
    public interface ICategoryRepository
    {
        Task<bool> BulkCreateAsync(List<CategoryModel> categoryModels);
        Task<List<CategoryModel>> GetAllAsync();
        Task<bool> BulkDeleteAsync();
        Task<CategoryModel> CreateAsync(CategoryModel category);

        Task<bool> DeleteAsync(Guid id);
    }
}