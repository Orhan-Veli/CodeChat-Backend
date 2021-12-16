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
        Task<IResult<bool>> CreateAsync(MessageModel model);
        Task<IResult<MessageModel>> GetAsync(Guid id);
        Task<IResult<List<MessageModel>>> GetAllAsync(Guid id);
        Task<IResult<MessageModel>> UpdateAsync(MessageModel model);
        Task<IResult<bool>> DeleteAsync(Guid id);
        Task<IResult<bool>> CreateMessageUserAsync(ReportedMessageModel reportedMessageModel);
        Task<IResult<List<MessageModel>>> GetAllReportedMessageAsync();
        Task<IResult<long>> GetAllMessageCountAsync();
        Task<IResult<long>> GetAllReportedMessageCountAsync();
    }
}