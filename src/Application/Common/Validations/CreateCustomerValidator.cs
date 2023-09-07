using FluentValidation;
using Models.Requests;

namespace Application.Common.Validations;

public class CreateCustomerValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.CustomerID)
            .MinimumLength(5).WithMessage("{PropertyName} Minimo 5 caracteres")
            .MaximumLength(5).WithMessage("{PropertyName} Maximo 5 caracteres")
            .NotEmpty().WithMessage("{PropertyName} No debe ser vacío")
            .NotNull().WithMessage("{PropertyName} No debe ser vacío");

        RuleFor(x => x.Address)
            .NotEmpty()
            .NotNull()
            .MinimumLength(1)
            .WithMessage("{PropertyName} Obligatorio");
    }
}