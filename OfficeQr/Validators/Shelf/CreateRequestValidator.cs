using FluentValidation;
using OfficeQr.Dtos.Shelf;

namespace OfficeQr.Validators.Shelf;


public class CreateRequestValidator : AbstractValidator<CreateRequest>
{
    public CreateRequestValidator ()
    {
        RuleFor(req => req.Capacity)
            .NotEmpty().WithMessage("Shelf capacity cannot be empty when creating");
        
        RuleFor(req => req.CabinetId)
            .NotEmpty().WithMessage("Shelf has to be an Cabinet id when creating");
        
    }
}