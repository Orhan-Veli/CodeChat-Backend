using System.Collections.Generic;
using System.Threading.Tasks;
using Message.Dal.Model;
namespace Message.Dal.Abstract
{
    public interface IRabbitMqUserRepository
    {
        Task Consumer(List<string> onlineUsers);

        Task Reciever();
    }
}