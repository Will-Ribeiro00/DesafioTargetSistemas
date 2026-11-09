using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using DesafioTargetSistemas.Application.UseCases.Sales.Register;
using DesafioTargetSistemas.Communication.Requests;
using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Domain.Enums;
using DesafioTargetSistemas.Domain.Services;
using DesafioTargetSistemas.Exception;
using DesafioTargetSistemas.Exception.ExceptionsBase;
using Moq;
using Shouldly;

namespace UseCase.Test.Sales.Register;

public class RegisterSaleUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        // Arrange
        var seller = SellerBuilder.Build();

        var request = RequestRegisterSaleJsonBuilder.Build();
        request.SellerId = seller.Id;

        var products = CreateProductsFor(request);
        var initialStocks = products.Select(p => p.CurrentStock).ToList();

        var expectedTotal = request.Items.Sum(i => i.Quantity * products.Single(p => p.Code == i.Code).Price);
        var expectedCommission = CommissionCalculator.Calculate(expectedTotal);

        var saleRepository = new SaleRepositoryBuilder();
        var useCase = CreateUseCase(saleRepository, seller, products);

        // Act
        var result = await useCase.Execute(request);

        // Assert - response
        result.TotalPrice.ShouldBe(expectedTotal);
        result.CommissionPercentage.ShouldBe(expectedCommission.Percentage);
        result.CommissionAmount.ShouldBe(expectedCommission.Amount);

        // Assert - sale in database
        var sale = saleRepository.AddedSale;
        sale.ShouldNotBeNull();
        sale.SellerId.ShouldBe(seller.Id);
        sale.Items.Count.ShouldBe(request.Items.Count);
        sale.StockMovements.Count.ShouldBe(request.Items.Count);
        sale.StockMovements.ShouldAllBe(m => m.Type == StockMovementType.Out);
        sale.AccountsReceivable.ShouldHaveSingleItem();
        sale.AccountsReceivable.Single().Amount.ShouldBe(expectedTotal);
        sale.AccountsReceivable.Single().DueDate.ShouldBe(request.DueDate);

        // Assert - stock movements
        for (var i = 0; i < products.Count; i++)
            products[i].CurrentStock.ShouldBe(initialStocks[i] - request.Items[i].Quantity);
    }

    [Fact]
    public async Task ErrorSellerNotFound()
    {
        // Arrange
        var request = RequestRegisterSaleJsonBuilder.Build();

        var saleRepository = new SaleRepositoryBuilder();
        var useCase = CreateUseCase(saleRepository);

        // Act
        Func<Task> act = async () => await useCase.Execute(request);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldHaveSingleItem();
        exception.GetErrorMessages().ShouldContain(ResourceMessageException.SELLER_NOT_FOUND);
        saleRepository.AddedSale.ShouldBeNull();
    }

    [Fact]
    public async Task ErrorProductNotFound()
    {
        // Arrange
        var seller = SellerBuilder.Build();

        var request = RequestRegisterSaleJsonBuilder.Build();
        request.SellerId = seller.Id;

        var saleRepository = new SaleRepositoryBuilder();
        var useCase = CreateUseCase(saleRepository, seller);

        // Act
        Func<Task> act = async () => await useCase.Execute(request);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldHaveSingleItem();
        exception.GetErrorMessages().ShouldContain(ResourceMessageException.PRODUCT_NOT_FOUND);
        saleRepository.AddedSale.ShouldBeNull();
    }

    [Fact]
    public async Task ErrorInsufficientStock()
    {
        // Arrange
        var seller = SellerBuilder.Build();

        var request = RequestRegisterSaleJsonBuilder.Build();
        request.SellerId = seller.Id;

        var products = CreateProductsFor(request);
        products[0].CurrentStock = request.Items[0].Quantity - 1;

        var saleRepository = new SaleRepositoryBuilder();
        var useCase = CreateUseCase(saleRepository, seller, products);

        // Act
        Func<Task> act = async () => await useCase.Execute(request);

        // Assert
        var exception = await act.ShouldThrowAsync<InsufficientStockException>();
        exception.GetErrorMessages().ShouldHaveSingleItem();
        exception.GetErrorMessages().ShouldContain(ResourceMessageException.INSUFFICIENT_STOCK);
        saleRepository.AddedSale.ShouldBeNull();
    }

    [Fact]
    public async Task ErrorInvalidRequest()
    {
        // Arrange
        var request = RequestRegisterSaleJsonBuilder.Build(0);

        var saleRepository = new SaleRepositoryBuilder();
        var useCase = CreateUseCase(saleRepository);

        // Act
        Func<Task> act = async () => await useCase.Execute(request);

        // Assert
        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldHaveSingleItem();
        exception.GetErrorMessages().ShouldContain(ResourceMessageException.SALE_ITEMS_REQUIRED);
        saleRepository.AddedSale.ShouldBeNull();
    }

    [Fact]
    public async Task SuccessAfterConcurrencyConflict()
    {
        // Arrange
        var seller = SellerBuilder.Build();

        var request = RequestRegisterSaleJsonBuilder.Build();
        request.SellerId = seller.Id;

        var products = CreateProductsFor(request);

        var sellerRepository = new SellerRepositoryBuilder();
        sellerRepository.GetById(seller);

        var productRepository = new ProductRepositoryBuilder();
        foreach (var product in products)
            productRepository.GetByCodeFromDatabase(product);

        var saleRepository = new SaleRepositoryBuilder();
        var unitOfWork = UnitOfWorkBuilder.BuildWithConflictOnFirstCommit();

        var useCase = new RegisterSaleUseCase(sellerRepository.Build(), productRepository.Build(), saleRepository.Build(), unitOfWork.Object);

        var expectedTotal = request.Items.Sum(i => i.Quantity * products.Single(p => p.Code == i.Code).Price);

        // Act
        var result = await useCase.Execute(request);

        // Assert
        result.TotalPrice.ShouldBe(expectedTotal);

        var movements = saleRepository.AddedSale!.StockMovements.ToList();

        for (var i = 0; i < products.Count; i++)
            movements[i].StockBalance.ShouldBe(products[i].CurrentStock - request.Items[i].Quantity);

        unitOfWork.Verify(u => u.Commit(), Times.Exactly(2));
        unitOfWork.Verify(u => u.Discard(), Times.Once);
    }

    private static List<Product> CreateProductsFor(RequestRegisterSaleJson request)
    {
        return [.. request.Items.Select((item, index) =>
        {
            var product = ProductBuilder.Build();
            product.Id = index + 1;
            product.Code = item.Code;
            return product;
        })];
    }

    private static RegisterSaleUseCase CreateUseCase(SaleRepositoryBuilder saleRepository, Seller? seller = null, List<Product>? products = null)
    {
        var sellerRepository = new SellerRepositoryBuilder();
        var productRepository = new ProductRepositoryBuilder();

        if (seller is not null)
            sellerRepository.GetById(seller);

        if (products is not null)
        {
            foreach (var product in products)
                productRepository.GetByCode(product);
        }

        return new RegisterSaleUseCase(
            sellerRepository.Build(),
            productRepository.Build(),
            saleRepository.Build(),
            UnitOfWorkBuilder.Build());
    }
}
