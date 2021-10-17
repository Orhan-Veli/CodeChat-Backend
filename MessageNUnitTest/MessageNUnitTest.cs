using NUnit.Framework;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Net.Http;
using Message.Controllers;
using Message.Business.Abstract;
using Message.Business.Concrete;
using Nest;
using Message.Dal.Model;
using Message.Dal.Abstract;
using Message.Dal.Concrete;
using Message.Utilities.Concrete;
using Message.Utilities.Abstract;
using FluentValidation;
using FluentValidation.TestHelper;
using Message.Validation;
using Moq;
using Microsoft.Extensions.Configuration;
namespace MessageNUnitTest
{
    [TestFixture]
    public class Tests
    {
        private MessageParameters _messageParameters;
        private MessageModelValidation _messageModelValidation;
        private MessageModel _messageModelFalse;
        private MessageModel _messageModelTrue;
        private string _indexName;
        private Dictionary<string,string> settings;
        private IConfiguration _configuration;
        [SetUp]
        public void Setup()
        {
            _messageParameters = new MessageParameters();
            _messageModelValidation = new MessageModelValidation();
            _messageModelTrue =new MessageModel {
                Id=Guid.Parse(_messageParameters.Id),
                CategoryId = Guid.Parse(_messageParameters.CategoryId),
                UserId = Guid.Parse(_messageParameters.UserId),
                CategoryName=_messageParameters.CategoryName,
                Text =_messageParameters.Text,               
                UserName = _messageParameters.UserName
            };
            _messageModelFalse = new MessageModel{ 
                Id=Guid.Parse(_messageParameters.Id),
                CategoryId = Guid.Parse(_messageParameters.CategoryId),
                UserId = Guid.Parse(_messageParameters.UserId),
                CategoryName="",
                Text=_messageParameters.Text,               
                UserName = ""
            };  
            settings = new Dictionary<string,string>
            {
                {"elasticsearchserver:Message","message"}
            };
            _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
            _indexName = "user";         
        }

        [Test]
        public void CreateMessageReturnsTrue()
        {           
            var result = _messageModelValidation.Validate(_messageModelTrue).IsValid;        
            Assert.AreEqual(true,result);
        }
        [Test]
        public void CreateMessageReturnsFalse()
        {          
            var result = _messageModelValidation.Validate(_messageModelFalse).IsValid;
            Assert.AreEqual(false,result);
        }

        [Test]
        public async Task GetMessageReturnTrue()
        {
        var mockElastic = new Mock<IElasticService>();
        var mockRabbitMq = new Mock<IRabbitMqService>();
        mockElastic.Setup(x=>x.GetAsync(Guid.Parse(_messageParameters.Id)).Result.Response).Returns(HttpStatusCode.Ok);
        var controller = new MessageController(mockElastic.Object,mockRabbitMq.Object);           
        var response = await controller.GetAsync(Guid.Parse(_messageParameters.Id));      
        Assert.IsInstanceOf<Microsoft.AspNetCore.Mvc.ObjectResult>(response);
        }

        [Test]
        public async Task GetMessageReturnFalse()
        {
        var mockElastic = new Mock<IElasticService>();
        var mockRabbitMq = new Mock<IRabbitMqService>();
        mockElastic.Setup(x=>x.GetAsync(Guid.Parse(_messageParameters.Empty)).Result.Response).Returns(HttpStatusCode.BadRequest);
        var controller = new MessageController(mockElastic.Object,mockRabbitMq.Object);           
        var response = await controller.GetAsync(Guid.Parse(_messageParameters.Empty));      
        Assert.IsInstanceOf<Microsoft.AspNetCore.Mvc.ObjectResult>(response);
        }

        [Test]
        public async Task GetAllMessageReturnTrue()
        {
        var mockElastic = new Mock<IElasticService>();
        var mockRabbitMq = new Mock<IRabbitMqService>();
        mockElastic.Setup(x=>x.GetAllAsync(Guid.Parse(_messageParameters.Id)).Result.Response).Returns(HttpStatusCode.Ok);
        var controller = new MessageController(mockElastic.Object,mockRabbitMq.Object);           
        var response = await controller.GetAllAsync(Guid.Parse(_messageParameters.Id));      
        Assert.IsInstanceOf<Microsoft.AspNetCore.Mvc.ObjectResult>(response);
        }

        [Test]
        public async Task GetAllMessageReturnFalse()
        {
        var mockElastic = new Mock<IElasticService>();
        var mockRabbitMq = new Mock<IRabbitMqService>();
        mockElastic.Setup(x=>x.GetAllAsync(Guid.Parse(_messageParameters.Empty)).Result.Response).Returns(HttpStatusCode.BadRequest);
        var controller = new MessageController(mockElastic.Object,mockRabbitMq.Object);           
        var response = await controller.GetAllAsync(Guid.Parse(_messageParameters.Empty));      
        Assert.IsInstanceOf<Microsoft.AspNetCore.Mvc.ObjectResult>(response);
        }

       [Test]
        public async Task DeleteMessageReturnTrue()
        {
        var mockElastic = new Mock<IElasticService>();
        var mockRabbitMq = new Mock<IRabbitMqService>();
        mockElastic.Setup(x=>x.DeleteAsync(Guid.Parse(_messageParameters.Id)).Result.Response).Returns(HttpStatusCode.NoContent);    
        var controller = new MessageController(mockElastic.Object,mockRabbitMq.Object);           
        var response = await controller.DeleteAsync(Guid.Parse(_messageParameters.Id));      
        Assert.IsInstanceOf<Microsoft.AspNetCore.Mvc.StatusCodeResult>(response);      
        }

         [Test]
        public async Task DeleteMessageReturnFalse()
        {
        var mockElastic = new Mock<IElasticService>();
        var mockRabbitMq = new Mock<IRabbitMqService>();
        mockElastic.Setup(x=>x.DeleteAsync(Guid.Parse(_messageParameters.Empty)).Result.Response).Returns(HttpStatusCode.BadRequest);
        var controller = new MessageController(mockElastic.Object,mockRabbitMq.Object);           
        var response = await controller.DeleteAsync(Guid.Parse(_messageParameters.Empty));      
        Assert.IsInstanceOf<Microsoft.AspNetCore.Mvc.StatusCodeResult>(response);
        }
    }
}