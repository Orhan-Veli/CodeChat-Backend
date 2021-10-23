using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Message.Dal.Model;
using Message.Dal.Abstract;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Message.Dal.SignalRHub;
using Message.Business.Concrete;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System.Runtime.Serialization.Formatters.Binary;
using Serilog;
namespace Message.Dal.Concrete
{
    public class RabbitMqUserRepository : IRabbitMqUserRepository
    {
        private readonly ConnectionFactory connectionFactory;
        private readonly IHubContext<UserHub> _userHubs;
        private readonly IConfiguration _configuration;   
        
        private readonly IServiceProvider _serviceProvider;
        public RabbitMqUserRepository(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
    
            connectionFactory = new ConnectionFactory()
            {
                UserName = _configuration["Rabbitmq:UserName"],
                Password = _configuration["Rabbitmq:Password"],
                VirtualHost =_configuration["Rabbitmq:VirtualHost"],
                HostName = _configuration["Rabbitmq:HostName"],
                Port = Convert.ToInt32(_configuration["Rabbitmq:Port"]),
                Uri = new Uri(_configuration["Rabbitmq:con"]) 
            };
             _serviceProvider=serviceProvider;
        }
        public async Task Consumer(List<string> onlinerUsers)//burada ekleme yapıyor
        {
            using (IConnection connection = connectionFactory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                channel.QueueDeclare(
                queue: "CodeChat-Users",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
                );
                var message = JsonConvert.SerializeObject(onlinerUsers);                          
                var body = Encoding.UTF8.GetBytes(message);
               

                channel.BasicPublish
                (
                    exchange: "",
                    routingKey: "CodeChat-Users",
                    basicProperties: null,
                    body: body
                );
            }
        }

        public async Task Reciever()//
        {
            using (IConnection connection = connectionFactory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                try
                {                
                channel.QueueDeclare(
                queue: "CodeChat-Users",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
                );   
                var consumer = new EventingBasicConsumer(channel);               
                consumer.Received += async (model, ea) =>
                {
                    var _userHub = (IHubContext<UserHub>)_serviceProvider.GetService(typeof(IHubContext<UserHub>));
                    var body = ea.Body.ToArray();
                    var onlineUsers = Encoding.UTF8.GetString(body);                                                             
                    await _userHub.Clients.All.SendAsync("UserConnected",onlineUsers);
                };
                
                channel.BasicConsume
                (
                    queue: "CodeChat-Users",
                    autoAck: true,
                    consumer: consumer
                );
                }
                catch (Exception ex)
                {
                     Log.Logger.Information("Rabbitmq error"+ ex.Message + DateTime.Now);
                }
            }
        }
    }
}