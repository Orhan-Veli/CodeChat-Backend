using Message.Business.Abstract;
using System.Threading.Tasks;
using Message.Dal.Model;
using Message.Dal.Abstract;
namespace Message.Business.Concrete
{
    public class RabbitMqService : IRabbitMqService
    {
        private readonly IRabbitMqRepository _rabbit;
        public RabbitMqService(IRabbitMqRepository rabbit)
        {
            _rabbit = rabbit;
        }
        public async Task Consumer(MessageModel model)
        {
            await _rabbit.Consumer(model);
        }
        public async Task Reciever()
        {
            await _rabbit.Reciever();
        }
    }
}