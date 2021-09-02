using FluentValidation;
using Message.Dal.Model;
using System;
namespace Message.Validation
{
    public class MessageModelValidation : AbstractValidator<MessageModel>
    {
        public MessageModelValidation()
        {
            RuleFor(x => x.Text).NotEmpty().NotNull();
            RuleFor(x => x.CategoryId).NotEqual(Guid.Empty);
            RuleFor(x => x.CategoryName).NotNull().NotEmpty();
        }
    }
}