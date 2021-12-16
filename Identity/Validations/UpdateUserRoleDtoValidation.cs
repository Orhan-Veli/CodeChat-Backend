using System;
using FluentValidation;
using Identity.Dtos;
namespace Identity.Validations
{
    public class UpdateUserRoleDtoValidation : AbstractValidator<UpdateUserRoleDto>
    {
        public UpdateUserRoleDtoValidation()
        {
            RuleFor(x=>x.UserId).NotNull().NotEmpty().WithMessage("UserId required.");
            RuleFor(x=>x.UserRole).NotNull().NotEmpty().WithMessage("UserName required");
        }   
    }
}