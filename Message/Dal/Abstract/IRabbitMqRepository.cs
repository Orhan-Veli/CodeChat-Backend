using System.Threading.Tasks;
using System.Collections.Generic;
using Message.Dal.Model;
namespace Message.Dal.Abstract
{
    public interface IRabbitMqRepository<T>
    {
        Task Consumer(T model);

        Task Reciever();

    
    }
}