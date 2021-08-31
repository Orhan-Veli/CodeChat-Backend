using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Category.Dal.Model;
namespace Category.Dal.Repositories.Abstract
{
    public interface IRepository
    {
        Task<bool> Create(List<CategoryModel> model);
        Task<List<CategoryModel>> GetAll();
        Task<bool> Delete();
    }
}