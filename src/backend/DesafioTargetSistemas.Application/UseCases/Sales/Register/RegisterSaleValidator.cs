using DesafioTargetSistemas.Communication.Requests;
using DesafioTargetSistemas.Domain.ValueObjects;
using DesafioTargetSistemas.Exception;
using FluentValidation;

namespace DesafioTargetSistemas.Application.UseCases.Sales.Register
{
    public class RegisterSaleValidator : AbstractValidator<RequestRegisterSaleJson>
    {
        public RegisterSaleValidator()
        {
            RuleFor(x => x.SellerId)
                .GreaterThan(0)
                .WithMessage(ResourceMessageException.INVALID_SELLER_ID);

            RuleFor(x => x.Items)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(ResourceMessageException.SALE_ITEMS_REQUIRED)
                .Must(items => items.Select(i => i.Code).Distinct(StringComparer.OrdinalIgnoreCase).Count() == items.Count)
                .WithMessage(ResourceMessageException.DUPLICATED_PRODUCTS_IN_SALE);

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.Code)
                    .NotEmpty().WithMessage(ResourceMessageException.PRODUCT_CODE_REQUIRED)
                    .MaximumLength(DesafioTargetSistemasRuleConstants.PRODUCT_CODE_MAX_LENGTH)
                    .WithMessage(ResourceMessageException.PRODUCT_CODE_TOO_LONG);

                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0)
                    .WithMessage(ResourceMessageException.INVALID_QUANTITY);
            });

            RuleFor(x => x.DueDate)
                .Must(date => date >= Today)
                .WithMessage(ResourceMessageException.DUE_DATE_IN_THE_PAST)
                .Must(date => date <= Today.AddDays(DesafioTargetSistemasRuleConstants.SALE_MAX_DUE_DAYS))
                .WithMessage(ResourceMessageException.DUE_DATE_TOO_FAR);
        }

        private static DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
    }
}
