using Message.Dal.Model;
using System;
using System.Collections.Generic;
namespace Message.Dal.Model
{
    public class RabbitMqModel
    {
        public RabbitMqModel()
        {
            OnlineUserModels = new List<OnlineUserModel>();
        }
        public List<OnlineUserModel> OnlineUserModels { get; set; }

        public MessageModel MessageModels { get; set; }
    }
}