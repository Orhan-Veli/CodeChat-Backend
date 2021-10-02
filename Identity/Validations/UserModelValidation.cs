using System;
using FluentValidation;
using Identity.Model;
namespace Identity.Validations
{
    public class UserModelValidation : AbstractValidator<UserModel>
    {
        public UserModelValidation()
        {
            RuleFor(x =>x.UserName).NotEmpty().NotNull().WithMessage("Name are is empty.");
            RuleFor(x=>x.Email).NotNull().NotEmpty().EmailAddress().WithMessage("Email address is required.");
            RuleFor(x=>x.Password).NotNull().NotEmpty().WithMessage("Password is empty");
        }   
    }
}