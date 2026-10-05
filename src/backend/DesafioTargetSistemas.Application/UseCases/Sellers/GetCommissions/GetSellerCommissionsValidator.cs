using DesafioTargetSistemas.Communication.Requests;
using DesafioTargetSistemas.Exception;
using FluentValidation;

namespace DesafioTargetSistemas.Application.UseCases.Sellers.GetCommissions
{
    public class GetSellerCommissionsValidator : AbstractValidator<RequestSellerCommissionsJson>
    {
        public GetSellerCommissionsValidator()
        {
            RuleFor(x => x.To)
                .Must((request, to) => request.From <= to)
                .When(x => x.From.HasValue && x.To.HasValue)
                .WithMessage(ResourceMessageException.INVALID_DATE_RANGE);
        }
    }
}
