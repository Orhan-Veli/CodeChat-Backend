using Nest;
using Message.Dal.Model;
namespace Message.Extensions
{
    public static class CreateIndexDescriptorExtensions
    {
        public static CreateIndexDescriptor MessageMapping(this CreateIndexDescriptor descriptor)
        {
            return descriptor.Map<MessageModel>
            (M => M.Properties
            (T => T.Keyword(K => K.Name(N => N.CategoryId))
            .Text(T => T.Name(N => N.CategoryName))
            .Keyword(T => T.Name(N => N.Id))
            .Keyword(T => T.Name(N => N.UserId))
            .Text(T => T.Name(N => N.Text))
            .Text(T => T.Name(N => N.UserName))
            .Text(T=> T.Name(N => N.CreatedOn))
            ));
        }
        public static CreateIndexDescriptor MessageUserMapping(this CreateIndexDescriptor descriptor)
        {
            return descriptor.Map<OnlineUserModel>
            (M => M.Properties
            (T => T.Text(K => K.Name(N => N.UserConnection))
            .Text(T => T.Name(N => N.UserName))            
            ));
        }
        public static CreateIndexDescriptor ReportedUserMapping(this CreateIndexDescriptor descriptor)
        {
            return descriptor.Map<ReportedMessageModel>
            (M => M.Properties
            (T => T.Keyword(K => K.Name(N => N.MessageId))
            .Text(T => T.Name(N => N.UserId))            
            ));
        }
    }
}