using FluentValidation;
using OfficeQr.Dtos.Item;

namespace OfficeQr.Validators.Item;

public class CreateRequestValidator : AbstractValidator<CreateRequest>
{
    public CreateRequestValidator()
    {
        RuleFor(req => req.Name)
            .NotEmpty().WithMessage("Item name cannot be empty")
            .MinimumLength(2)
            .MaximumLength(255);
        
    }
    
}