using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Message.Dal.Abstract;
using Message.Utilities.Abstract;
using Message.Utilities.Concrete;
using Message.Dal.Model;
using Message.Business.Abstract;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Message.Business.Concrete
{
    public class ElasticService : IElasticService
    {
        private readonly IEntityRepository<MessageModel> _elasticRepository;
        private readonly string _indexName;
        public ElasticService(IConfiguration configuration,IEntityRepository<MessageModel> elasticRepository)
        { 
            _elasticRepository = elasticRepository;
            _indexName = configuration["elasticsearchserver:Message"].ToString();
        }
        public async Task<IResult<bool>> Create(MessageModel model)
        {           
            var result = await _elasticRepository.Create(model.Id,model,_indexName);
            if (!result)
            {
                Log.Logger.Information("Model could not be created." + DateTime.Now);
                return new Result<bool>(false, "Model could not be created.");
            }
            Log.Logger.Information("Model created." + DateTime.Now);
            return new Result<bool>(result);
        }
        public async Task<IResult<MessageModel>> Get(Guid id)
        {
            if (id == Guid.Empty)
            {
                Log.Logger.Information("Id is empty." + DateTime.Now);
                return new Result<MessageModel>(false, "Id is empty.");
            }
            var result = await _elasticRepository.Get(id,_indexName);
            if (result == null)
            {
                return new Result<MessageModel>(false, "No data found.");
            }
            return new Result<MessageModel>(true, result);
        }
        public async Task<IResult<List<MessageModel>>> GetAll(Guid id)
        {
            if(id == Guid.Empty)
            {
                return new Result<List<MessageModel>>(false, "Id is empty.");
            }
            var result = await _elasticRepository.GetAll(_indexName);
            if (result == null)
            {
                Log.Logger.Information("List of data is null." + DateTime.Now);
                return new Result<List<MessageModel>>(true,new List<MessageModel>());
            }
            result = result.Where(x=>x.CategoryId == id).ToList();            
            return new Result<List<MessageModel>>(true, result);
        }
        public async Task<IResult<MessageModel>> Update(MessageModel model)
        {
            var result = await _elasticRepository.Update(model.Id,model,_indexName);
            if (result == null)
            {
                Log.Logger.Information("Update could not be done." + DateTime.Now);
                return new Result<MessageModel>(false, "Update could not be done.");
            }
            Log.Logger.Information("Update done." + DateTime.Now);
            return new Result<MessageModel>(true, result);
        }
        public async Task<IResult<bool>> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                Log.Logger.Information("Id is empty." + DateTime.Now);
                return new Result<bool>(false, "Id is empty.");
            }
            var result = await _elasticRepository.Delete(id,_indexName);
            if (!result)
            {
                Log.Logger.Information("Delete could not be done." + DateTime.Now);
                return new Result<bool>(false, "Delete could not be done.");
            }
            Log.Logger.Information("Delete done." + DateTime.Now);
            return new Result<bool>(true);
        }
    }
}