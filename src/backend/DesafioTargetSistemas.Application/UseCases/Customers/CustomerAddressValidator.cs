using DesafioTargetSistemas.Communication.Requests;
using DesafioTargetSistemas.Domain.ValueObjects;
using DesafioTargetSistemas.Exception;
using FluentValidation;

namespace DesafioTargetSistemas.Application.UseCases.Customers;

public class CustomerAddressValidator : AbstractValidator<RequestCustomerAddressJson>
{
    private static readonly HashSet<string> States =
        [
            "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO", "MA", "MT", "MS", "MG", "PA",
            "PB", "PR", "PE", "PI", "RJ", "RN", "RS", "RO", "RR", "SC", "SP", "SE", "TO"
        ];

    public CustomerAddressValidator()
    {
        RuleFor(x => x.ZipCode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ResourceMessageException.ADDRESS_INVALID_ZIP_CODE)
            .Matches("^[0-9]{8}$").WithMessage(ResourceMessageException.ADDRESS_INVALID_ZIP_CODE);

        RuleFor(x => x.Street)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ResourceMessageException.ADDRESS_STREET_REQUIRED)
            .MaximumLength(DesafioTargetSistemasRuleConstants.ADDRESS_STREET_MAX_LENGTH).WithMessage(ResourceMessageException.ADDRESS_STREET_TOO_LONG);

        RuleFor(x => x.Number)
            .MaximumLength(DesafioTargetSistemasRuleConstants.ADDRESS_NUMBER_MAX_LENGTH).WithMessage(ResourceMessageException.ADDRESS_NUMBER_TOO_LONG);

        RuleFor(x => x.Complement)
            .MaximumLength(DesafioTargetSistemasRuleConstants.ADDRESS_COMPLEMENT_MAX_LENGTH).WithMessage(ResourceMessageException.ADDRESS_COMPLEMENT_TOO_LONG);

        RuleFor(x => x.District)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ResourceMessageException.ADDRESS_DISTRICT_REQUIRED)
            .MaximumLength(DesafioTargetSistemasRuleConstants.ADDRESS_DISTRICT_MAX_LENGTH).WithMessage(ResourceMessageException.ADDRESS_DISTRICT_TOO_LONG);

        RuleFor(x => x.City)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ResourceMessageException.ADDRESS_CITY_REQUIRED)
            .MaximumLength(DesafioTargetSistemasRuleConstants.ADDRESS_CITY_MAX_LENGTH).WithMessage(ResourceMessageException.ADDRESS_CITY_TOO_LONG);

        RuleFor(x => x.State)
            .Must(state => state is not null && States.Contains(state.ToUpperInvariant())).WithMessage(ResourceMessageException.ADDRESS_INVALID_STATE);
    }
}
