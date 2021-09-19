using System;
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
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
namespace Message.Dal.Concrete
{
    public class RabbitMqRepository : IRabbitMqRepository
    {
         private readonly IHubContext<ChatHub> _chatHubs;
        private readonly IConfiguration _configuration;      
        public RabbitMqRepository(IConfiguration configuration,IHubContext<ChatHub> chatHub)
        {
            _configuration = configuration;
            _chatHubs = chatHub;
            
        }
        public async Task Consumer(MessageModel model)
        {
            var factory = new ConnectionFactory();
            factory.UserName = "orhan";
            factory.Password = "123456";
            factory.VirtualHost = "/";
            factory.HostName = "localhost";
            factory.Port = 5672;

            factory.Uri = new Uri(_configuration["Rabbitmq:con"]);
            using (IConnection connection = factory.CreateConnection())
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
            var factory = new ConnectionFactory();
            factory.UserName = "orhan";
            factory.Password = "123456";
            factory.VirtualHost = "/";
            factory.HostName = "localhost";
            factory.Uri = new Uri(_configuration["Rabbitmq:con"]);
            using (IConnection connection = factory.CreateConnection())
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
                var _chatHub= new ChatHub(_chatHubs);
                var consumer = new EventingBasicConsumer(channel);               
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);                     
                   //var messageModel = JsonConvert.DeserializeObject<string>(message);
                  await _chatHub.SendMessage(message);
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
                    
                }
               
            }
        }
    }
}