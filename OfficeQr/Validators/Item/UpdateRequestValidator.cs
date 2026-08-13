using System.Data;
using FluentValidation;
using OfficeQr.Dtos.Item;

namespace OfficeQr.Validators.Item;


public class UpdateRequestValidator : AbstractValidator<UpdateRequest>
{
    public UpdateRequestValidator()
    {
        RuleFor(req => req.Id)
            .NotEmpty().WithMessage("Id con not be empty when item updating");
        
        RuleFor(req => req.Name)
            .MaximumLength(255).WithMessage("Name has to be shorter than 255 chars")
            .MinimumLength(2).WithMessage("Name has to be longer than 2 chars");

    }
}