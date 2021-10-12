using Message.Dal.Abstract;
using Message.Dal.Model;
using Nest;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
namespace Message.Dal.Concrete
{
    public class ElasticRepository<T> : IEntityRepository<T> where T: class
    {
        private readonly IElasticClient _elasticClient;      
        public ElasticRepository(IConfiguration configuration, IElasticClient elasticClient)
        {          
            _elasticClient = elasticClient;
        }
        public async Task<bool> Create(Guid id,T model,string _indexName)
        {
            var response = await _elasticClient.CreateAsync(model, x => x.Index(_indexName).Id(id));
            return response.IsValid;
        }
        public async Task<T> Update(Guid id, T model,string _indexName)
        {
            var response = await _elasticClient.UpdateAsync<T>(id, x => x.Index(_indexName).Doc(model));
            return model;
        }
        public async Task<T> Get(Guid id,string _indexName)
        {
            var response = await _elasticClient.GetAsync<T>(id, x => x.Index(_indexName));
            return response.Source;
        }
        public async Task<List<T>> GetAll(string _indexName)
        {
            var response = await _elasticClient.SearchAsync<T>(x => 
            x.Index(_indexName)
            .From(0)
            .Size(1000)            
            ); 
            return response.Documents.ToList();
        }
        public async Task<bool> Delete(Guid id,string _indexName)
        {
            var response = await _elasticClient.DeleteAsync<T>(id, x => x.Index(_indexName));
            return response.IsValid;
        }

        public async Task<bool> CreateUser(string id,OnlineUserModel onlineUserModel, string _indexName)
        {
            var response = await _elasticClient.CreateAsync(onlineUserModel, x => x.Index(_indexName).Id(id));
            return response.IsValid;
        }

        public async Task<bool> DeleteUser(string id,string _indexName)
        {
            var response = await _elasticClient.DeleteAsync<T>(id, x => x.Index(_indexName));
            return response.IsValid;
        }

        public async Task<OnlineUserModel> GetUser(string name,string _indexName)
        {
             var response = await _elasticClient.SearchAsync<OnlineUserModel>(x => x.Index(_indexName).Query(q => q.Match(x=>x.Query(name))));             
            return response.Documents.FirstOrDefault();
        }
    }
}