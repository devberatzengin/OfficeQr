using FluentValidation;
using OfficeQr.Dtos.Shelf;

namespace OfficeQr.Validators.Shelf;


public class UpdateRequestValidator : AbstractValidator<UpdateRequest>
{
    public UpdateRequestValidator()
    {
        RuleFor(req => req.Id)
            .NotEmpty().WithMessage("Id can not null when shelf updating");
        
        
    }
}