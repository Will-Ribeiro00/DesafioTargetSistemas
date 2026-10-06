using DesafioTargetSistemas.Communication.Requests;
using DesafioTargetSistemas.Exception;
using FluentValidation;

namespace DesafioTargetSistemas.Application.UseCases.Login.DoLogin
{
    public class DoLoginValidator : AbstractValidator<RequestLoginJson>
    {
        public DoLoginValidator()
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(ResourceMessageException.EMAIL_REQUIRED)
                .EmailAddress().WithMessage(ResourceMessageException.INVALID_EMAIL_FORMAT);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(ResourceMessageException.PASSWORD_REQUIRED);
        }
    }
}
