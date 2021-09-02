using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Message.Dal.Abstract;
using Message.Utilities.Abstract;
using Message.Utilities.Concrete;
using Message.Dal.Model;
using Message.Business.Abstract;
using Serilog;

namespace Message.Business.Concrete
{
    public class ElasticService : IElasticService
    {
        private readonly IEntityRepository<MessageModel> _elasticRepository;
        public ElasticService(IEntityRepository<MessageModel> elasticRepository)
        {
            _elasticRepository = elasticRepository;
        }
        public async Task<IResult<bool>> Create(MessageModel model)
        {
            model.Id = Guid.NewGuid();
            var result = await _elasticRepository.Create(model);
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
            var result = await _elasticRepository.Get(id);
            if (result == null)
            {
                return new Result<MessageModel>(false, "No data found.");
            }
            return new Result<MessageModel>(true, result);
        }
        public async Task<IResult<List<MessageModel>>> GetAll()
        {
            var result = await _elasticRepository.GetAll();
            if (result == null)
            {
                Log.Logger.Information("List of data is null." + DateTime.Now);
                return new Result<List<MessageModel>>(false, "List of data is null.");
            }
            return new Result<List<MessageModel>>(true, result);
        }
        public async Task<IResult<MessageModel>> Update(MessageModel model)
        {
            var result = await _elasticRepository.Update(model);
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
            var result = await _elasticRepository.Delete(id);
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