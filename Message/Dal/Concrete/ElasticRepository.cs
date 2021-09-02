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
    public class ElasticRepository : IEntityRepository<MessageModel>, IElasticRepository
    {
        private readonly IElasticClient _elasticClient;
        private readonly string _indexName;
        public ElasticRepository(IConfiguration configuration, IElasticClient elasticClient)
        {
            _indexName = configuration["elasticsearchserver:indexName"].ToString();
            _elasticClient = elasticClient;
        }
        public async Task<bool> Create(MessageModel model)
        {
            var response = await _elasticClient.CreateAsync(model, x => x.Index(_indexName).Id(model.CategoryId));
            return response.IsValid;
        }
        public async Task<MessageModel> Update(MessageModel model)
        {
            var response = await _elasticClient.UpdateAsync<MessageModel>(model.Id, x => x.Index(_indexName).Doc(model));
            return model;
        }
        public async Task<MessageModel> Get(Guid id)
        {
            var response = await _elasticClient.GetAsync<MessageModel>(id, x => x.Index(_indexName));
            return response.Source;
        }
        public async Task<List<MessageModel>> GetAll()
        {
            var response = await _elasticClient.SearchAsync<MessageModel>(x => x.Index(_indexName).Scroll("1m"));
            return response.Documents.ToList();
        }
        public async Task<bool> Delete(Guid id)
        {
            var response = await _elasticClient.DeleteAsync<MessageModel>(id, x => x.Index(_indexName));
            return response.IsValid;
        }
    }
}