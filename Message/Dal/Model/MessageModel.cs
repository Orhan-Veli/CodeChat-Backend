using System;
using System.Globalization;
namespace Message.Dal.Model
{
    public class MessageModel
    {
        public MessageModel()
        {
            Id = Guid.NewGuid();
            CreatedOn = DateTime.Now;
        }
        public Guid Id { get; set; } 
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}