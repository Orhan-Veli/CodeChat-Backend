using FluentValidation;
using Message.Dal.Model;
using System;
namespace Message.Validation
{
    public class MessageModelValidation : AbstractValidator<MessageModel>
    {
        public MessageModelValidation()
        {
            RuleFor(x => x.Text).NotEmpty().NotNull().WithMessage("Text is empty.");
            RuleFor(x => x.CategoryId).NotEqual(Guid.Empty).WithMessage("CategoryId is empty.");
            RuleFor(x => x.CategoryName).NotNull().NotEmpty().WithMessage("CategoryName is empty.");
            RuleFor(x=>x.UserId).NotEqual(Guid.Empty).WithMessage("UserId is empty.");
            RuleFor(x=> x.UserName).NotNull().NotEmpty().WithMessage("UserName is empty.");
        }
    }
}