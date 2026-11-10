using DesafioTargetSistemas.Domain.Services;
using DesafioTargetSistemas.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.AccountsReceivable.GetOpen;

public class GetOpenAccountsReceivableTest : DesafioTargetSistemasClassFixture
{
    private const string METHOD = "api/hml/accountsreceivable/open";

    private readonly CustomWebApplicationFactory _factory;
    private readonly string _token;

    public GetOpenAccountsReceivableTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _factory = factory;
        _token = factory.GetToken();
    }

    [Fact]
    public async Task Success()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var expectedAccounts = _factory.GetAccountsReceivable()
            .Where(a => a.PaymentDate is null)
            .OrderBy(a => a.DueDate)
            .ToList();

        // Act
        var response = await DoGet(METHOD, _token);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var accounts = responseData.RootElement;

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        accounts.GetArrayLength().ShouldBe(expectedAccounts.Count);

        for (var i = 0; i < expectedAccounts.Count; i++)
        {
            var expected = expectedAccounts[i];
            var expectedDays = InterestCalculator.DaysOverdue(expected.DueDate, today);
            var expectedInterest = InterestCalculator.Calculate(expected.Amount, expectedDays);

            accounts[i].GetProperty("id").GetInt32().ShouldBe(expected.Id);
            accounts[i].GetProperty("amount").GetDecimal().ShouldBe(expected.Amount);
            accounts[i].GetProperty("dueDate").GetString().ShouldBe(expected.DueDate.ToString("yyyy-MM-dd"));
            accounts[i].GetProperty("daysOverdue").GetInt32().ShouldBe(expectedDays);
            accounts[i].GetProperty("interest").GetDecimal().ShouldBe(expectedInterest);
            accounts[i].GetProperty("totalAmount").GetDecimal().ShouldBe(expected.Amount + expectedInterest);
        }
    }

    [Fact]
    public async Task SuccessPaidAccountIsNotListed()
    {
        // Arrange
        var paidAccount = _factory.GetAccountsReceivable().Single(a => a.PaymentDate is not null);

        // Act
        var response = await DoGet(METHOD, _token);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var ids = responseData.RootElement.EnumerateArray()
            .Select(account => account.GetProperty("id").GetInt32())
            .ToList();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        ids.ShouldNotContain(paidAccount.Id);
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