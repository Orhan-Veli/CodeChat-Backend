using NUnit.Framework;
using System;
using System.Threading.Tasks;
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
namespace MessageNUnitTest
{
    [TestFixture]
    public class Tests
    {
        private MessageParameters _messageParameters;
        private MessageModelValidation _messageModelValidation;
        private MessageModel _messageModelFalse;
        private MessageModel _messageModelTrue;
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
                Text=_messageParameters.Text,               
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
        var mockElastic = new Mock<IEntityRepository<MessageModel>>();
        var mockElasticDb= new Mock<IElasticClient>();
        //mockElasticDb.Setup(x => x.Get<MessageModel>(It.IsAny<Guid.Parse(_messageParameters.UserId),x=>x.Index("message"))).Verifiable();
        mockElastic.Setup(x=>x.Get(Guid.Parse(_messageParameters.UserId)));
        IElasticService _elasticService = new ElasticService(mockElastic.Object);           
        var response = await _elasticService.Get(Guid.Parse(_messageParameters.UserId));
        mockElasticDb.Verify();
        Assert.AreEqual(true,response.Success);
        }
        [Test]
        public async Task GetMessageReturnFalse()
        {
        var mockElastic = new Mock<IEntityRepository<MessageModel>>();
        mockElastic.Setup(x=>x.Get(Guid.Parse(_messageParameters.Empty)));
        IElasticService _elasticService = new ElasticService(mockElastic.Object);           
        var response = await _elasticService.Get(Guid.Parse(_messageParameters.UserId));
        Assert.AreEqual(false,response.Success);
        }

        [Test]
        public async Task GetAllMessageReturnTrue()
        {
        var mockElastic = new Mock<IEntityRepository<MessageModel>>();
        mockElastic.Setup(x=>x.Get(Guid.Parse(_messageParameters.CategoryId)));
        IElasticService _elasticService = new ElasticService(mockElastic.Object);           
        var response = await _elasticService.Get(Guid.Parse(_messageParameters.UserId));
        Assert.AreEqual(true,response.Success);
        }

        [Test]
        public async Task GetAllMessageReturnFalse()
        {
        var mockElastic = new Mock<IEntityRepository<MessageModel>>();
        mockElastic.Setup(x=>x.Get(Guid.Parse(_messageParameters.CategoryId)));
        IElasticService _elasticService = new ElasticService(mockElastic.Object);           
        var response = await _elasticService.Get(Guid.Parse(_messageParameters.UserId));
        Assert.AreEqual(false,response.Success);
        }

       [Test]
        public async Task DeleteMessageReturnTrue()
        {
        var mockElastic = new Mock<IEntityRepository<MessageModel>>();
        mockElastic.Setup(x=>x.Get(Guid.Parse(_messageParameters.UserId)));
        IElasticService _elasticService = new ElasticService(mockElastic.Object);           
        var response = await _elasticService.Get(Guid.Parse(_messageParameters.UserId));
        Assert.AreEqual(true,response.Success);
        }

         [Test]
        public async Task DeleteMessageReturnFalse()
        {
        var mockElastic = new Mock<IEntityRepository<MessageModel>>();
        mockElastic.Setup(x=>x.Get(Guid.Parse(_messageParameters.UserId)));
        IElasticService _elasticService = new ElasticService(mockElastic.Object);           
        var response = await _elasticService.Get(Guid.Parse(_messageParameters.UserId));
        Assert.AreEqual(false,response.Success);
        }
    }
}