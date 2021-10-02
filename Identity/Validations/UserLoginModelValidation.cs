using System;
using FluentValidation;
using Identity.Model;
namespace Identity.Validations
{
    public class UserLoginModelValidation : AbstractValidator<UserLoginModel>
    {
        public UserLoginModelValidation()
        {
            RuleFor(x=>x.Email).NotNull().NotEmpty().EmailAddress().WithMessage("Email address is required.");
            RuleFor(x=>x.Password).NotNull().NotEmpty().WithMessage("Password is empty");
        }   
    }
}