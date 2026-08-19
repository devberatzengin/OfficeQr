using FluentValidation;
using OfficeQr.Dtos.Item;

namespace OfficeQr.Validators.Item;

public class ReturnRequestValidator : AbstractValidator<ReturnRequest>
{
    public ReturnRequestValidator()
    {
        RuleFor(req => req.ItemId)
            .NotEmpty().WithMessage("Id can not be null")
            .NotEqual(Guid.Empty).WithMessage("Guid id can not be null");

    /*
        RuleFor(req => req.ShelfId)
            .NotEmpty().WithMessage("Id can not be null")
            .NotEqual(Guid.Empty).WithMessage("Guid id can not be null");

        RuleFor(req => req.UserId)
            .NotEmpty().WithMessage("Id can not be null")
            .NotEqual(Guid.Empty).WithMessage("Guid id can not be null");
    */
    }
}