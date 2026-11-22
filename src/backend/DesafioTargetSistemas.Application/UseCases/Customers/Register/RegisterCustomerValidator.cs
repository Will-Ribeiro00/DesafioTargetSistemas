using DesafioTargetSistemas.Communication.Requests;
using DesafioTargetSistemas.Domain.Enums;
using DesafioTargetSistemas.Domain.Services;
using DesafioTargetSistemas.Domain.ValueObjects;
using DesafioTargetSistemas.Exception;
using FluentValidation;

namespace DesafioTargetSistemas.Application.UseCases.Customers.Register;

public class RegisterCustomerValidator : AbstractValidator<RequestRegisterCustomerJson>
{
    public RegisterCustomerValidator()
    {
        RuleFor(x => x.PersonType)
            .IsInEnum().WithMessage(ResourceMessageException.CUSTOMER_INVALID_PERSON_TYPE);

        RuleFor(x => x.Document)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ResourceMessageException.CUSTOMER_DOCUMENT_REQUIRED)
            .Must(BrazilianDocument.IsValidCpf)
                .When(x => x.PersonType == PersonType.Individual, ApplyConditionTo.CurrentValidator)
                .WithMessage(ResourceMessageException.CUSTOMER_INVALID_CPF)
            .Must(document => BrazilianDocument.IsValidCnpj(document.ToUpperInvariant()))
                .When(x => x.PersonType == PersonType.Company, ApplyConditionTo.CurrentValidator)
                .WithMessage(ResourceMessageException.CUSTOMER_INVALID_CNPJ);

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ResourceMessageException.CUSTOMER_NAME_REQUIRED)
            .MaximumLength(DesafioTargetSistemasRuleConstants.CUSTOMER_NAME_MAX_LENGTH).WithMessage(ResourceMessageException.CUSTOMER_NAME_TOO_LONG);

        RuleFor(x => x.TradeName)
            .MaximumLength(DesafioTargetSistemasRuleConstants.CUSTOMER_TRADE_NAME_MAX_LENGTH).WithMessage(ResourceMessageException.CUSTOMER_TRADE_NAME_TOO_LONG);

        RuleFor(x => x.StateRegistration)
            .MaximumLength(DesafioTargetSistemasRuleConstants.CUSTOMER_STATE_REGISTRATION_MAX_LENGTH).WithMessage(ResourceMessageException.CUSTOMER_STATE_REGISTRATION_TOO_LONG);

        RuleFor(x => x)
            .Must(x => x.PersonType == PersonType.Company
                || (string.IsNullOrWhiteSpace(x.TradeName) && string.IsNullOrWhiteSpace(x.StateRegistration)))
            .WithMessage(ResourceMessageException.CUSTOMER_COMPANY_ONLY_FIELDS);

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .EmailAddress().WithMessage(ResourceMessageException.INVALID_EMAIL_FORMAT)
            .MaximumLength(DesafioTargetSistemasRuleConstants.CUSTOMER_EMAIL_MAX_LENGTH).WithMessage(ResourceMessageException.CUSTOMER_EMAIL_TOO_LONG)
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Phone)
            .Matches("^[0-9]{10,11}$")
            .WithMessage(ResourceMessageException.CUSTOMER_INVALID_PHONE)
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x.SellerId)
            .GreaterThan(0)
            .WithMessage(ResourceMessageException.INVALID_SELLER_ID);

        RuleFor(x => x.Address)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ResourceMessageException.ADDRESS_REQUIRED)
            .SetValidator(new CustomerAddressValidator());
    }
}
