using System.Data;
using FluentValidation;
using OfficeQr.Dtos.Cabinet;

namespace OfficeQr.Validators.Cabinet;

public class UpdateRequestValidator : AbstractValidator<UpdateRequest>
{
    public UpdateRequestValidator ()
    {
        RuleFor(req => req.Capacity)
            .NotEmpty().WithMessage("Cabinet capacity can not empty when updating");
    }
} 