using DesafioTargetSistemas.Communication.Requests;
using DesafioTargetSistemas.Domain.Enums;
using DesafioTargetSistemas.Domain.ValueObjects;
using DesafioTargetSistemas.Exception;
using FluentValidation;

namespace DesafioTargetSistemas.Application.UseCases.StockMovements.Register
{
    public class RegisterStockMovementValidator : AbstractValidator<RequestRegisterStockMovementJson>
    {
        public RegisterStockMovementValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage(ResourceMessageException.PRODUCT_CODE_REQUIRED)
                .MaximumLength(DesafioTargetSistemasRuleConstants.PRODUCT_CODE_MAX_LENGTH)
                .WithMessage(ResourceMessageException.PRODUCT_CODE_TOO_LONG);

            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage(ResourceMessageException.INVALID_MOVEMENT_TYPE);

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage(ResourceMessageException.INVALID_QUANTITY);

            RuleFor(x => x.Description)
                .MaximumLength(DesafioTargetSistemasRuleConstants.STOCK_MOVEMENT_DESCRIPTION_MAX_LENGTH)
                .WithMessage(ResourceMessageException.DESCRIPTION_TOO_LONG);
        }
    }
}
