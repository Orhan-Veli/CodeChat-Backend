using Message.Dal.Model;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace Message.Business.Abstract
{
    public interface IRabbitMqService
    {
        Task Consumer(MessageModel model);
        Task Reciever();
    }
}