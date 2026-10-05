using DesafioTargetSistemas.Communication.Requests;
using DesafioTargetSistemas.Communication.Responses;
using DesafioTargetSistemas.Domain.Repositories;
using DesafioTargetSistemas.Exception.ExceptionsBase;

namespace DesafioTargetSistemas.Application.UseCases.Sellers.GetCommissions
{
    public class GetSellerCommissionsUseCase(ISaleRepository saleRepository) : IGetSellerCommissionsUseCase
    {
        private readonly ISaleRepository _saleRepository = saleRepository;

        public async Task<List<ResponseSellerCommissionJson>> Execute(RequestSellerCommissionsJson request)
        {
            Validate(request);

            var from = request.From?.ToDateTime(TimeOnly.MinValue);
            var toExclusive = request.To?.AddDays(1).ToDateTime(TimeOnly.MinValue);

            var summaries = await _saleRepository.GetCommissionsBySeller(from, toExclusive);

            return [.. summaries.Select(s => new ResponseSellerCommissionJson
            {
                SellerId = s.SellerId,
                SellerName = s.SellerName,
                SalesCount = s.SalesCount,
                TotalSold = s.TotalSold,
                TotalCommission = s.TotalCommission
            })];
        }

        private static void Validate(RequestSellerCommissionsJson request)
        {
            var result = new GetSellerCommissionsValidator().Validate(request);

            if (!result.IsValid)
                throw new ErrorOnValidationException([.. result.Errors.Select(e => e.ErrorMessage)]);
        }
    }
}
