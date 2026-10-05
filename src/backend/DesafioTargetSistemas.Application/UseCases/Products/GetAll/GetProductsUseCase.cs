using DesafioTargetSistemas.Communication.Responses;
using DesafioTargetSistemas.Domain.Repositories;

namespace DesafioTargetSistemas.Application.UseCases.Products.GetAll
{
    public class GetProductsUseCase(IProductRepository productRepository) : IGetProductsUseCase
    {
        private readonly IProductRepository _productRepository = productRepository;

        public async Task<List<ResponseProductJson>> Execute()
        {
            var products = await _productRepository.GetAll();

            return [.. products.Select(p => new ResponseProductJson
            {
                Code = p.Code,
                Description = p.Description,
                Price = p.Price,
                CurrentStock = p.CurrentStock
            })];
        }
    }
}
