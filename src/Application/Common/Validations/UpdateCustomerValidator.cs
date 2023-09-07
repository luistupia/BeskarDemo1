using FluentValidation;
using Models.Requests;

namespace Application.Common.Validations;

public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerRequest>
{
    public UpdateCustomerValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .NotNull();
        
        RuleFor(x => x.ContactName)
            .NotEmpty()
            .NotNull();
        
        RuleFor(x => x.Address)
            .NotEmpty()
            .NotNull();
    }
}