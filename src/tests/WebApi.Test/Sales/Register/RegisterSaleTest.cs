using CommonTestUtilities.Requests;
using DesafioTargetSistemas.Domain.Services;
using DesafioTargetSistemas.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.Sales.Register;

public class RegisterSaleTest : DesafioTargetSistemasClassFixture
{
    private const string METHOD = "api/hml/sales";

    private readonly CustomWebApplicationFactory _factory;
    private readonly string _token;
    private readonly string _productCode;
    private readonly int _sellerId;

    public RegisterSaleTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _factory = factory;
        _token = factory.GetToken();
        _productCode = factory.GetProductCode();
        _sellerId = factory.GetSellerId();
    }

    [Fact]
    public async Task Success()
    {
        // Arrange
        var request = RequestRegisterSaleJsonBuilder.Build(1);
        request.SellerId = _sellerId;
        request.Items[0].Code = _productCode;

        var stockBefore = _factory.GetProductCurrentStock();
        var expectedTotal = request.Items[0].Quantity * _factory.GetProductPrice();
        var expectedCommission = CommissionCalculator.Calculate(expectedTotal);

        // Act
        var response = await DoPost(METHOD, request, _token);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        responseData.RootElement.GetProperty("id").GetInt32().ShouldBeGreaterThan(0);
        responseData.RootElement.GetProperty("totalPrice").GetDecimal().ShouldBe(expectedTotal);
        responseData.RootElement.GetProperty("commissionPercentage").GetDecimal().ShouldBe(expectedCommission.Percentage);
        responseData.RootElement.GetProperty("commissionAmount").GetDecimal().ShouldBe(expectedCommission.Amount);

        _factory.GetProductCurrentStock().ShouldBe(stockBefore - request.Items[0].Quantity);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task ErrorSellerNotFound(string culture)
    {
        // Arrange
        var request = RequestRegisterSaleJsonBuilder.Build(1);
        request.SellerId = 999;
        request.Items[0].Code = _productCode;

        // Act
        var response = await DoPost(METHOD, request, _token, culture);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
        var expectedMessage = ResourceMessageException.ResourceManager
            .GetString(nameof(ResourceMessageException.SELLER_NOT_FOUND), new CultureInfo(culture));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        errors.ShouldHaveSingleItem();
        errors.ShouldContain(e => e.GetString()!.Equals(expectedMessage));
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task ErrorProductNotFound(string culture)
    {
        // Arrange
        var request = RequestRegisterSaleJsonBuilder.Build(1);
        request.SellerId = _sellerId;
        request.Items[0].Code = "0000";

        // Act
        var response = await DoPost(METHOD, request, _token, culture);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
        var expectedMessage = ResourceMessageException.ResourceManager
            .GetString(nameof(ResourceMessageException.PRODUCT_NOT_FOUND), new CultureInfo(culture));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        errors.ShouldHaveSingleItem();
        errors.ShouldContain(e => e.GetString()!.Equals(expectedMessage));
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task ErrorInsufficientStock(string culture)
    {
        // Arrange
        var stockBefore = _factory.GetProductCurrentStock();

        var request = RequestRegisterSaleJsonBuilder.Build(1);
        request.SellerId = _sellerId;
        request.Items[0].Code = _productCode;
        request.Items[0].Quantity = stockBefore + 1;

        // Act
        var response = await DoPost(METHOD, request, _token, culture);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
        var expectedMessage = ResourceMessageException.ResourceManager
            .GetString(nameof(ResourceMessageException.INSUFFICIENT_STOCK), new CultureInfo(culture));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        errors.ShouldHaveSingleItem();
        errors.ShouldContain(e => e.GetString()!.Equals(expectedMessage));

        _factory.GetProductCurrentStock().ShouldBe(stockBefore);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task ErrorDueDateInThePast(string culture)
    {
        // Arrange
        var request = RequestRegisterSaleJsonBuilder.Build(1);
        request.SellerId = _sellerId;
        request.Items[0].Code = _productCode;
        request.DueDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);

        // Act
        var response = await DoPost(METHOD, request, _token, culture);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
        var expectedMessage = ResourceMessageException.ResourceManager
            .GetString(nameof(ResourceMessageException.DUE_DATE_IN_THE_PAST), new CultureInfo(culture));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        errors.ShouldHaveSingleItem();
        errors.ShouldContain(e => e.GetString()!.Equals(expectedMessage));
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task ErrorWithoutToken(string culture)
    {
        // Arrange
        var request = RequestRegisterSaleJsonBuilder.Build(1);

        // Act
        var response = await DoPost(METHOD, request, culture: culture);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
        var expectedMessage = ResourceMessageException.ResourceManager
            .GetString(nameof(ResourceMessageException.WITHOUT_TOKEN), new CultureInfo(culture));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        errors.ShouldHaveSingleItem();
        errors.ShouldContain(e => e.GetString()!.Equals(expectedMessage));
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task ErrorInvalidToken(string culture)
    {
        // Arrange
        var request = RequestRegisterSaleJsonBuilder.Build(1);

        // Act
        var response = await DoPost(METHOD, request, token: "tokenInvalido", culture: culture);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
        var expectedMessage = ResourceMessageException.ResourceManager
            .GetString(nameof(ResourceMessageException.INVALID_TOKEN), new CultureInfo(culture));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        errors.ShouldHaveSingleItem();
        errors.ShouldContain(e => e.GetString()!.Equals(expectedMessage));
    }
}
