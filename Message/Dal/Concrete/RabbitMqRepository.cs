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
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
namespace Message.Dal.Concrete
{
    public class RabbitMqRepository : Hub, IRabbitMqRepository
    {
        private readonly IConfiguration _configuration;
        public RabbitMqRepository(IConfiguration configuration)
        {
            _configuration = configuration;
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
                queue: "Message",
                durable: false,
                exclusive: true,
                autoDelete: true,
                arguments: null
                );

                var message = JsonConvert.SerializeObject(model);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish
                (
                    exchange: "",
                    routingKey: "Message",
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
                channel.QueueDeclare(
                queue: "Message",
                durable: true,
                exclusive: true,
                autoDelete: true,
                arguments: null
                );

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.Span;
                    var message = Encoding.UTF8.GetString(body);
                    var messageModel = JsonConvert.DeserializeObject<MessageModel>(message);
                    Clients.All.SendAsync("ReceiveMessage", messageModel);
                };
                channel.BasicConsume
                (
                    queue: "Message",
                    autoAck: true,
                    consumer: consumer
                );
            }
        }

    }
}