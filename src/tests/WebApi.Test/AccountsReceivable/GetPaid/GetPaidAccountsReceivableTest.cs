using DesafioTargetSistemas.Domain.Services;
using DesafioTargetSistemas.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.AccountsReceivable.GetPaid;

public class GetPaidAccountsReceivableTest : DesafioTargetSistemasClassFixture
{
    private const string METHOD = "api/hml/accountsreceivable/paid";

    private readonly CustomWebApplicationFactory _factory;
    private readonly string _token;

    public GetPaidAccountsReceivableTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _factory = factory;
        _token = factory.GetToken();
    }

    [Fact]
    public async Task Success()
    {
        // Arrange: a única conta paga do seed
        var paid = _factory.GetAccountsReceivable().Single(a => a.PaymentDate is not null);
        var paymentDate = paid.PaymentDate!.Value;
        var paidAmount = paid.PaidAmount!.Value;

        // Act
        var response = await DoGet(METHOD, _token);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var accounts = responseData.RootElement;

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        accounts.GetArrayLength().ShouldBe(1);

        accounts[0].GetProperty("saleId").GetInt32().ShouldBe(paid.SaleId);
        accounts[0].GetProperty("amount").GetDecimal().ShouldBe(paid.Amount);
        accounts[0].GetProperty("dueDate").GetString().ShouldBe(paid.DueDate.ToString("yyyy-MM-dd"));
        accounts[0].GetProperty("paymentDate").GetString().ShouldBe(paymentDate.ToString("yyyy-MM-dd"));
        accounts[0].GetProperty("daysOverdue").GetInt32().ShouldBe(InterestCalculator.DaysOverdue(paid.DueDate, paymentDate));
        accounts[0].GetProperty("interest").GetDecimal().ShouldBe(paidAmount - paid.Amount);
        accounts[0].GetProperty("paidAmount").GetDecimal().ShouldBe(paidAmount);
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
