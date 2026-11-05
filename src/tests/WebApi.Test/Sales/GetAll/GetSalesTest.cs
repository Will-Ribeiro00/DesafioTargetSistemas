using DesafioTargetSistemas.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.Sales.GetAll;

public class GetSalesTest : DesafioTargetSistemasClassFixture
{
    private const string METHOD = "api/hml/sales";

    private readonly CustomWebApplicationFactory _factory;
    private readonly string _token;

    public GetSalesTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _factory = factory;
        _token = factory.GetToken();
    }

    [Fact]
    public async Task Success()
    {
        // Arrange
        var expectedSales = _factory.GetSales()
            .OrderByDescending(s => s.SaleDate)
            .ThenByDescending(s => s.Id)
            .ToList();

        // Act
        var response = await DoGet(METHOD, _token);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var sales = responseData.RootElement;

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        sales.GetArrayLength().ShouldBe(expectedSales.Count);

        for (var i = 0; i < expectedSales.Count; i++)
        {
            var expected = expectedSales[i];

            sales[i].GetProperty("id").GetInt32().ShouldBe(expected.Id);
            sales[i].GetProperty("saleDate").GetDateTime().ShouldBe(expected.SaleDate);
            sales[i].GetProperty("sellerName").GetString().ShouldBe(_factory.GetSellerName());
            sales[i].GetProperty("totalPrice").GetDecimal().ShouldBe(expected.TotalPrice);
            sales[i].GetProperty("commissionPercentage").GetDecimal().ShouldBe(expected.CommissionPercentage);
            sales[i].GetProperty("commissionAmount").GetDecimal().ShouldBe(expected.CommissionAmount);
        }
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