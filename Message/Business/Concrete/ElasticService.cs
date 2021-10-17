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
        private readonly IElasticRepository<MessageModel> _elasticRepository;
        private readonly string _indexName;
        public ElasticService(IConfiguration configuration,IElasticRepository<MessageModel> elasticRepository)
        { 
            _elasticRepository = elasticRepository;
            _indexName = configuration["elasticsearchserver:Message"].ToString();
        }
        public async Task<IResult<bool>> CreateAsync(MessageModel model)
        {           
            var result = await _elasticRepository.CreateAsync(model.Id,model,_indexName);
            if (!result)
            {
                Log.Logger.Information("Model could not be created." + DateTime.Now);
                return new Result<bool>(false, "Model could not be created.",HttpStatusCode.NotFound);
            }
            Log.Logger.Information("Model created." + DateTime.Now);
            return new Result<bool>(result,HttpStatusCode.Created);
        }
        public async Task<IResult<MessageModel>> GetAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                Log.Logger.Information("Id is empty." + DateTime.Now);
                return new Result<MessageModel>(false, "Id is empty.",HttpStatusCode.BadRequest);
            }
            var result = await _elasticRepository.GetAsync(id,_indexName);
            if (result == null)
            {
                return new Result<MessageModel>(false, "No data found.",HttpStatusCode.NotFound);
            }
            return new Result<MessageModel>(true, result,HttpStatusCode.Ok);
        }
        public async Task<IResult<List<MessageModel>>> GetAllAsync(Guid id)
        {
            if(id == Guid.Empty)
            {
                return new Result<List<MessageModel>>(false, "Id is empty.",HttpStatusCode.BadRequest);
            }
            var result = await _elasticRepository.GetAllAsync(_indexName);
            if (result == null)
            {
                Log.Logger.Information("List of data is null." + DateTime.Now);
                return new Result<List<MessageModel>>(true,new List<MessageModel>(),HttpStatusCode.Ok);
            }
            result = result.Where(x=>x.CategoryId == id).OrderBy(t => t.CreatedOn).ToList();            
            return new Result<List<MessageModel>>(true, result,HttpStatusCode.Ok);
        }
        public async Task<IResult<MessageModel>> UpdateAsync(MessageModel model)
        {
            var result = await _elasticRepository.UpdateAsync(model.Id,model,_indexName);
            if (result == null)
            {
                Log.Logger.Information("Update could not be done." + DateTime.Now);
                return new Result<MessageModel>(false, "Update could not be done.",HttpStatusCode.NotFound);
            }
            Log.Logger.Information("Update done." + DateTime.Now);
            return new Result<MessageModel>(true, result,HttpStatusCode.Ok);
        }
        public async Task<IResult<bool>> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                Log.Logger.Information("Id is empty." + DateTime.Now);
                return new Result<bool>(false, "Id is empty.",HttpStatusCode.BadRequest);
            }
            var result = await _elasticRepository.DeleteAsync(id,_indexName);
            if (!result)
            {
                Log.Logger.Information("Delete could not be done." + DateTime.Now);
                return new Result<bool>(false, "Delete could not be done.",HttpStatusCode.NotFound);
            }
            Log.Logger.Information("Delete done." + DateTime.Now);
            return new Result<bool>(true,HttpStatusCode.NoContent);
        }
    }
}