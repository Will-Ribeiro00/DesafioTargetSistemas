using DesafioTargetSistemas.Communication.Requests;
using DesafioTargetSistemas.Communication.Responses;
using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Domain.Enums;
using DesafioTargetSistemas.Domain.Repositories;
using DesafioTargetSistemas.Domain.Services;
using DesafioTargetSistemas.Domain.ValueObjects;
using DesafioTargetSistemas.Exception;
using DesafioTargetSistemas.Exception.ExceptionsBase;

namespace DesafioTargetSistemas.Application.UseCases.Sales.Register
{
    internal class RegisterSaleUseCase : IRegisterSaleUseCase
    {
        private readonly ISellerRepository _sellerRepository;
        private readonly IProductRepository _productRepository;
        private readonly ISaleRepository _saleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterSaleUseCase(
            ISellerRepository sellerRepository,
            IProductRepository productRepository,
            ISaleRepository saleRepository,
            IUnitOfWork unitOfWork)
        {
            _sellerRepository = sellerRepository;
            _productRepository = productRepository;
            _saleRepository = saleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseRegisteredSaleJson> Execute(RequestRegisterSaleJson request)
        {
            Validate(request);

            var seller = await _sellerRepository.GetById(request.SellerId)
                ?? throw new NotFoundException(ResourceMessageException.SELLER_NOT_FOUND);

            var sale = new Sale
            {
                SellerId = seller.Id,
                SaleDate = DateTime.UtcNow
            };

            foreach (var item in request.Items)
            {
                var product = await _productRepository.GetByCode(item.Code)
                    ?? throw new NotFoundException(ResourceMessageException.PRODUCT_NOT_FOUND);

                var newBalance = product.CurrentStock - item.Quantity;

                if (newBalance < 0)
                    throw new InsufficientStockException();

                product.CurrentStock = newBalance;

                sale.Items.Add(new SaleItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                });

                sale.StockMovements.Add(new StockMovement
                {
                    ProductId = product.Id,
                    Type = StockMovementType.Out,
                    Quantity = item.Quantity,
                    StockBalance = newBalance,
                    MovementDate = sale.SaleDate,
                    Description = DesafioTargetSistemasRuleConstants.STOCK_MOVEMENT_SALE_DESCRIPTION
                });
            }

            sale.TotalPrice = sale.Items.Sum(i => i.Quantity * i.UnitPrice);

            var commission = CommissionCalculator.Calculate(sale.TotalPrice);
            sale.CommissionPercentage = commission.Percentage;
            sale.CommissionAmount = commission.Amount;

            sale.AccountsReceivable.Add(new AccountReceivable
            {
                Amount = sale.TotalPrice,
                DueDate = request.DueDate
            });

            await _saleRepository.Add(sale);

            await _unitOfWork.Commit();

            return new ResponseRegisteredSaleJson
            {
                Id = sale.Id,
                TotalPrice = sale.TotalPrice,
                CommissionPercentage = sale.CommissionPercentage,
                CommissionAmount = sale.CommissionAmount
            };
        }

        private static void Validate(RequestRegisterSaleJson request)
        {
            var result = new RegisterSaleValidator().Validate(request);

            if (!result.IsValid)
                throw new ErrorOnValidationException([.. result.Errors.Select(e => e.ErrorMessage)]);
        }
    }
}
