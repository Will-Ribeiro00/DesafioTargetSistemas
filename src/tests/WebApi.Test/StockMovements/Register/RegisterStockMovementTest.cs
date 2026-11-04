using CommonTestUtilities.Requests;
using DesafioTargetSistemas.Domain.Enums;
using DesafioTargetSistemas.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.StockMovements.Register;

public class RegisterStockMovementTest : DesafioTargetSistemasClassFixture
{
    private const string METHOD = "api/hml/stockmovements";

    private readonly CustomWebApplicationFactory _factory;
    private readonly string _token;
    private readonly string _productCode;

    public RegisterStockMovementTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _factory = factory;
        _token = factory.GetToken();
        _productCode = factory.GetProductCode();
    }

    [Fact]
    public async Task Success()
    {
        // Arrange
        var request = RequestRegisterStockMovementJsonBuilder.Build();
        request.Code = _productCode;
        request.Type = StockMovementType.In;

        var stockBefore = _factory.GetProductCurrentStock();

        // Act
        var response = await DoPost(METHOD, request, _token);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        responseData.RootElement.GetProperty("id").GetInt32().ShouldBeGreaterThan(0);
        responseData.RootElement.GetProperty("stockBalance").GetInt32().ShouldBe(stockBefore + request.Quantity);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task ErrorProductNotFound(string culture)
    {
        // Arrange
        var request = RequestRegisterStockMovementJsonBuilder.Build();
        request.Code = "0000";

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
        var request = RequestRegisterStockMovementJsonBuilder.Build();
        request.Code = _productCode;
        request.Type = StockMovementType.Out;
        request.Quantity = _factory.GetProductCurrentStock() + 1;

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
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task ErrorInvalidQuantity(string culture)
    {
        // Arrange
        var request = RequestRegisterStockMovementJsonBuilder.Build();
        request.Quantity = 0;

        // Act
        var response = await DoPost(METHOD, request, _token, culture);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
        var expectedMessage = ResourceMessageException.ResourceManager
            .GetString(nameof(ResourceMessageException.INVALID_QUANTITY), new CultureInfo(culture));

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
        var request = RequestRegisterStockMovementJsonBuilder.Build();

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
        var request = RequestRegisterStockMovementJsonBuilder.Build();

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
