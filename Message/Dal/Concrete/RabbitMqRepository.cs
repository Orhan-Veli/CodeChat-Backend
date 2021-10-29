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
    public class RabbitMqRepository : IRabbitMqRepository
    {
        private readonly IHubContext<ChatHub> _chatHubs;
        private readonly IConfiguration _configuration;      
        private readonly ConnectionFactory connectionFactory;
        private readonly ChatHub _chatHub;

        public RabbitMqRepository(IConfiguration configuration,IHubContext<ChatHub> chatHub)
        {
            _configuration = configuration;
            _chatHubs = chatHub;
            connectionFactory = new ConnectionFactory()
            {
                UserName = _configuration["Rabbitmq:UserName"],
                Password = _configuration["Rabbitmq:Password"],
                VirtualHost =_configuration["Rabbitmq:VirtualHost"],
                HostName = _configuration["Rabbitmq:HostName"],
                Port = Convert.ToInt32(_configuration["Rabbitmq:Port"]),
                Uri = new Uri(_configuration["Rabbitmq:con"])
            };
            _chatHub = new ChatHub(_chatHubs); 
        }
        public async Task Consumer(MessageModel model)
        {
            using (IConnection connection = connectionFactory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                channel.QueueDeclare(
                queue: "CodeChat-Messages",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
                );                              
                var message = JsonConvert.SerializeObject(model);                          
                var body = Encoding.UTF8.GetBytes(message);              

                channel.BasicPublish
                (
                    exchange: "",
                    routingKey: "CodeChat-Messages",
                    basicProperties: null,
                    body: body
                );
            }
        }
        public async Task Reciever()
        {
            using (IConnection connection = connectionFactory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                try
                {                
                channel.QueueDeclare(
                queue: "CodeChat-Messages",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
                );             
                var consumer = new EventingBasicConsumer(channel);               
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);                      
                    var messageModel = JsonConvert.DeserializeObject<MessageModel>(message);                                           
                    await _chatHub.SendMessage(messageModel.CategoryId, message);
                };
                
                channel.BasicConsume
                (
                    queue: "CodeChat-Messages",
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