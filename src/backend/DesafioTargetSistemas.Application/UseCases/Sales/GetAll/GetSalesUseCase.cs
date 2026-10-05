using DesafioTargetSistemas.Communication.Responses;
using DesafioTargetSistemas.Domain.Repositories;

namespace DesafioTargetSistemas.Application.UseCases.Sales.GetAll
{
    public class GetSalesUseCase(ISaleRepository saleRepository) : IGetSalesUseCase
    {
        private readonly ISaleRepository _saleRepository = saleRepository;

        public async Task<List<ResponseSaleJson>> Execute()
        {
            var sales = await _saleRepository.GetAll();

            return [.. sales.Select(s => new ResponseSaleJson
            {
                Id = s.Id,
                SaleDate = s.SaleDate,
                SellerName = s.Seller.Name,
                TotalPrice = s.TotalPrice,
                CommissionPercentage = s.CommissionPercentage,
                CommissionAmount = s.CommissionAmount
            })];
        }
    }
}
