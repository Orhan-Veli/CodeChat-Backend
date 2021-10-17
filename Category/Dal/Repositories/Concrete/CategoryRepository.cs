using Newtonsoft.Json;
using Category.Dal.Repositories.Abstract;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Category.Dal.Model;
using System.Text;
namespace Category.Dal.Repositories.Concrete
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IDistributedCache _distributed;
        private readonly IConfiguration _configuration;
        private readonly string _key;
        public CategoryRepository(IDistributedCache distributed, IConfiguration configuration)
        {
            _distributed = distributed;
            _configuration = configuration;
            _key = _configuration["rediscache:Key"].ToString();
        }
        public async Task<bool> BulkCreateAsync(List<CategoryModel> CategoryModels)
        {
            var data = JsonConvert.SerializeObject(CategoryModels);
            var dataByte = Encoding.UTF8.GetBytes(data);
            await _distributed.SetAsync(_key, dataByte);
            return true;
        }
        public async Task<List<CategoryModel>> GetAllAsync()
        {
            string result = await _distributed.GetStringAsync(_key);
            if (result == null)
            {
                return new List<CategoryModel>();
            }
            return JsonConvert.DeserializeObject<List<CategoryModel>>(result);
        }
        public async Task<bool> BulkDeleteAsync()
        {
            await _distributed.RemoveAsync(_key);
            return true;
        }

        public async Task<CategoryModel> CreateAsync(CategoryModel category)
        {
            var allCategoryModel = await GetAllAsync();
            allCategoryModel.Add(category);
            await BulkDeleteAsync();
            await BulkCreateAsync(allCategoryModel);
            return category;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var allCategoryModel = await GetAllAsync();
            allCategoryModel.RemoveAll(x => x.Id == id);
            await BulkDeleteAsync();
            await BulkCreateAsync(allCategoryModel);
            return true;
        }
    }
}