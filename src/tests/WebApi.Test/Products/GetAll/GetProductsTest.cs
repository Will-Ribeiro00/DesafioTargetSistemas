using DesafioTargetSistemas.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.Products.GetAll;

public class GetProductsTest : DesafioTargetSistemasClassFixture
{
    private const string METHOD = "api/hml/products";

    private readonly CustomWebApplicationFactory _factory;
    private readonly string _token;

    public GetProductsTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _factory = factory;
        _token = factory.GetToken();
    }

    [Fact]
    public async Task Success()
    {
        // Act
        var response = await DoGet(METHOD, _token);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var products = responseData.RootElement;

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        products.GetArrayLength().ShouldBe(1);
        products[0].GetProperty("code").GetString().ShouldBe(_factory.GetProductCode());
        products[0].GetProperty("description").GetString().ShouldBe(_factory.GetProductDescription());
        products[0].GetProperty("price").GetDecimal().ShouldBe(_factory.GetProductPrice());
        products[0].GetProperty("currentStock").GetInt32().ShouldBe(_factory.GetProductCurrentStock());
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task ErrorWithoutToken(string culture)
    {
        // Act
        var response = await DoGet(METHOD, culture: culture);

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
        // Act
        var response = await DoGet(METHOD, token: "tokenInvalido", culture: culture);

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