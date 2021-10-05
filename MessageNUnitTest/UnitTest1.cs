using NUnit.Framework;
using System;
using System.Net.Http;
using Message.Controllers;
using Message.Business.Abstract;
using Message.Business.Concrete;
using System.Threading.Tasks;
using Message.Dal.Model;
using Message.Dal.Abstract;
using Message.Dal.Concrete;
using Message.Utilities.Concrete;
using Moq;
namespace MessageNUnitTest
{
    [TestFixture]
    public class Tests
    {
        private MessageParameters _messageParameters;
        
        [SetUp]
        public void Setup()
        {
            _messageParameters = new MessageParameters();
        }

        [Test]
        public async Task CreateMessageReturnsTrue()
        {           
            MessageModel messageModel = new MessageModel
            {
                Id=Guid.Parse(_messageParameters.Id),
                CategoryId = Guid.Parse(_messageParameters.CategoryId),
                UserId = Guid.Parse(_messageParameters.UserId),
                CategoryName=_messageParameters.CategoryName,
                Text=_messageParameters.Text,               
                UserName = _messageParameters.UserName
            };
            var mockElastic = new Mock<IEntityRepository<MessageModel>>();
            mockElastic.Setup(x=>x.Create(messageModel)).ReturnsAsync(true);
            IElasticService _elasticService = new ElasticService(mockElastic.Object);           
            var response = await _elasticService.Create(messageModel);
            Assert.AreEqual(true,response.Success);
        }
    }
}