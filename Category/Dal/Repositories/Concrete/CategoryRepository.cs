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
    public class CategoryRepository : IRepository
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
        public async Task<bool> Create(List<CategoryModel> category)
        {
            var data = JsonConvert.SerializeObject(category);
            var dataByte = Encoding.UTF8.GetBytes(data);
            await _distributed.SetAsync(_key, dataByte);
            return true;
        }

        public async Task<List<CategoryModel>> GetAll()
        {
            string result = await _distributed.GetStringAsync(_key);
            if (result == null)
            {
                return new List<CategoryModel>();
            }
            return JsonConvert.DeserializeObject<List<CategoryModel>>(result);
        }

        public async Task<bool> Delete()
        {
            await _distributed.RemoveAsync(_key);
            return true;
        }
    }
}