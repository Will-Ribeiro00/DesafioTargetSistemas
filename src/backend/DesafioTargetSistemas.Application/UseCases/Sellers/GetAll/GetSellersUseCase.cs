using DesafioTargetSistemas.Communication.Responses;
using DesafioTargetSistemas.Domain.Repositories;

namespace DesafioTargetSistemas.Application.UseCases.Sellers.GetAll
{
    public class GetSellersUseCase(ISellerRepository sellerRepository) : IGetSellersUseCase
    {
        private readonly ISellerRepository _sellerRepository = sellerRepository;

        public async Task<List<ResponseSellerJson>> Execute()
        {
            var sellers = await _sellerRepository.GetAll();

            return [.. sellers.Select(s => new ResponseSellerJson
            {
                Id = s.Id,
                Name = s.Name
            })];
        }
    }
}
