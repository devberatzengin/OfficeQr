using FluentValidation;
using OfficeQr.Dtos.Item;

namespace OfficeQr.Validators.Item;

public class CreateRequestValidator : AbstractValidator<CreateRequest>
{
    public CreateRequestValidator()
    {
        RuleFor(req => req.Name)
            .MaximumLength(255)
            .MinimumLength(2)
            .NotEmpty().WithMessage("Item name cannot be empty");
        
    }
    
}