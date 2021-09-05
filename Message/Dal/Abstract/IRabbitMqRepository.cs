using System.Threading.Tasks;
using Message.Dal.Model;
namespace Message.Dal.Abstract
{
    public interface IRabbitMqRepository
    {
        Task Consumer(MessageModel model);

        Task Reciever();
    }
}