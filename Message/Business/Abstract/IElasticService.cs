using System;
using Message.Utilities.Abstract;
using Message.Utilities.Concrete;
using Message.Dal.Model;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace Message.Business.Abstract
{
    public interface IElasticService
    {
        Task<IResult<bool>> Create(MessageModel model);
        Task<IResult<MessageModel>> Get(Guid id);
        Task<IResult<List<MessageModel>>> GetAll(Guid id);
        Task<IResult<MessageModel>> Update(MessageModel model);
        Task<IResult<bool>> Delete(Guid id);
    }
}