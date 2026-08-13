using System.IO.Compression;
using FluentValidation;
using OfficeQr.Dtos.Cabinet;

namespace OfficeQr.Validators.Cabinet;


public class CreateRequestValidator : AbstractValidator<CreateRequest>
{
    
    public CreateRequestValidator()
    {
        RuleFor(req => req.Capacity)
            .NotEmpty().WithMessage("Cabinet capacity can not empty when cabinet creating.");
    }

}