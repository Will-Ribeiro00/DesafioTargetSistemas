using DesafioTargetSistemas.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.Sellers.GetCommissions;

public class GetSellerCommissionsTest : DesafioTargetSistemasClassFixture
{
    private const string METHOD = "api/hml/sellers/commissions";

    private readonly CustomWebApplicationFactory _factory;
    private readonly string _token;

    public GetSellerCommissionsTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _factory = factory;
        _token = factory.GetToken();
    }

    [Fact]
    public async Task Success()
    {
        // Arrange
        var sales = _factory.GetSales();

        // Act
        var response = await DoGet(METHOD, _token);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var commissions = responseData.RootElement;

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        commissions.GetArrayLength().ShouldBe(1);
        commissions[0].GetProperty("sellerName").GetString().ShouldBe(_factory.GetSellerName());
        commissions[0].GetProperty("salesCount").GetInt32().ShouldBe(sales.Count);
        commissions[0].GetProperty("totalSold").GetDecimal().ShouldBe(sales.Sum(s => s.TotalPrice));
        commissions[0].GetProperty("totalCommission").GetDecimal().ShouldBe(sales.Sum(s => s.CommissionAmount));
    }

    [Fact]
    public async Task SuccessFilteredByPeriod()
    {
        // Arrange
        var octoberSales = _factory.GetSales().Where(s => s.SaleDate.Month == 10).ToList();

        // Act
        var response = await DoGet($"{METHOD}?from=2026-10-01&to=2026-10-31", _token);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var commissions = responseData.RootElement;

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        commissions.GetArrayLength().ShouldBe(1);
        commissions[0].GetProperty("salesCount").GetInt32().ShouldBe(octoberSales.Count);
        commissions[0].GetProperty("totalSold").GetDecimal().ShouldBe(octoberSales.Sum(s => s.TotalPrice));
        commissions[0].GetProperty("totalCommission").GetDecimal().ShouldBe(octoberSales.Sum(s => s.CommissionAmount));
    }

    [Fact]
    public async Task SuccessLastDayIncluded()
    {
        // Act: período de um dia só, o mesmo dia da venda das 15h30
        var response = await DoGet($"{METHOD}?from=2026-10-20&to=2026-10-20", _token);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var commissions = responseData.RootElement;

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        commissions.GetArrayLength().ShouldBe(1);
        commissions[0].GetProperty("salesCount").GetInt32().ShouldBe(1);
    }

    [Fact]
    public async Task SuccessEmpty()
    {
        // Act: um período sem nenhuma venda
        var response = await DoGet($"{METHOD}?from=2026-01-01&to=2026-01-31", _token);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        responseData.RootElement.GetArrayLength().ShouldBe(0);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task ErrorInvalidDateRange(string culture)
    {
        // Act
        var response = await DoGet($"{METHOD}?from=2026-10-31&to=2026-10-01", _token, culture);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
        var expectedMessage = ResourceMessageException.ResourceManager
            .GetString(nameof(ResourceMessageException.INVALID_DATE_RANGE), new CultureInfo(culture));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        errors.ShouldHaveSingleItem();
        errors.ShouldContain(e => e.GetString()!.Equals(expectedMessage));
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