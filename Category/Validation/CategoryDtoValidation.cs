using FluentValidation;
using Category.Dto;
namespace Category.Validation
{
    public class CategoryDtoValidation : AbstractValidator<CategoryDto>
    {
        public CategoryDtoValidation()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty();
            RuleFor(x => x.Image).NotNull().NotEmpty();
        }
    }
}