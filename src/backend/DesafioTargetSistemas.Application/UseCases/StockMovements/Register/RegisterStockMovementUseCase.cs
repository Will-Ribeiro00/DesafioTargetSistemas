using DesafioTargetSistemas.Communication.Requests;
using DesafioTargetSistemas.Communication.Responses;
using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Domain.Enums;
using DesafioTargetSistemas.Domain.Repositories;
using DesafioTargetSistemas.Exception;
using DesafioTargetSistemas.Exception.ExceptionsBase;

namespace DesafioTargetSistemas.Application.UseCases.StockMovements.Register
{
    public class RegisterStockMovementUseCase : IRegisterStockMovementUseCase
    {
        private readonly IProductRepository _productRepository;
        private readonly IStockMovementRepository _stockMovementRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterStockMovementUseCase(
            IProductRepository productRepository,
            IStockMovementRepository stockMovementRepository,
            IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _stockMovementRepository = stockMovementRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseRegisteredStockMovementJson> Execute(RequestRegisterStockMovementJson request)
        {
            Validate(request);

            var product = await _productRepository.GetByCode(request.Code)
                ?? throw new NotFoundException(ResourceMessageException.PRODUCT_NOT_FOUND);

            var newBalance = request.Type == StockMovementType.In
                ? product.CurrentStock + request.Quantity
                : product.CurrentStock - request.Quantity;

            if (newBalance < 0)
                throw new InsufficientStockException();

            product.CurrentStock = newBalance;
            _productRepository.Update(product);

            var movement = new StockMovement
            {
                ProductId = product.Id,
                Type = request.Type,
                Description = request.Description,
                Quantity = request.Quantity,
                StockBalance = newBalance,
                MovementDate = DateTime.UtcNow
            };

            await _stockMovementRepository.Add(movement);

            await _unitOfWork.Commit();

            return new ResponseRegisteredStockMovementJson
            {
                Id = movement.Id,
                StockBalance = newBalance
            };
        }

        private static void Validate(RequestRegisterStockMovementJson request)
        {
            var result = new RegisterStockMovementValidator().Validate(request);

            if (!result.IsValid)
                throw new ErrorOnValidationException([.. result.Errors.Select(e => e.ErrorMessage)]);
        }
    }
}
