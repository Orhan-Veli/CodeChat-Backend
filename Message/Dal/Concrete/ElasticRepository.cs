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
    public class ElasticRepository<T> : IElasticRepository<T> where T: class
    {
        private readonly IElasticClient _elasticClient;      
        public ElasticRepository(IConfiguration configuration, IElasticClient elasticClient)
        {          
            _elasticClient = elasticClient;
        }
        public async Task<bool> CreateAsync(Guid id,T model,string _indexName)
        {
            var response = await _elasticClient.CreateAsync(model, x => x.Index(_indexName).Id(id));
            return response.IsValid;
        }
        public async Task<T> UpdateAsync(Guid id, T model,string _indexName)
        {
            var response = await _elasticClient.UpdateAsync<T>(id, x => x.Index(_indexName).Doc(model));
            return model;
        }
        public async Task<T> GetAsync(Guid id,string _indexName)
        {
            var response = await _elasticClient.GetAsync<T>(id, x => x.Index(_indexName));
            return response.Source;
        }
        public async Task<List<T>> GetAllAsync(string _indexName)
        {
            var response = await _elasticClient.SearchAsync<T>(x => 
            x.Index(_indexName)
            .From(0)
            .Size(2000)           
            ); 
            return response.Documents.ToList();
        }
        public async Task<bool> DeleteAsync(Guid id,string _indexName)
        {
            var response = await _elasticClient.DeleteAsync<T>(id, x => x.Index(_indexName));
            return response.IsValid;
        }

        public async Task<List<T>> GetReportedMessagesAsync(List<Guid> messageIds,string _indexName)
        {
            var response = await _elasticClient.SearchAsync<T>(t=> t.Index(_indexName).Query(x=> x.Ids(x=>x.Values(messageIds))));
            return response.Documents.ToList();
        }
    }
}