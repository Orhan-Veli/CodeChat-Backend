using System;
namespace Message.Dal.Model
{
    public class ReportedMessageModel
    {
        public Guid MessageId { get; set; }

        public Guid UserId { get; set; }
    }
}