using DesafioTargetSistemas.Domain.Services;
using DesafioTargetSistemas.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.AccountsReceivable.Pay;

public class PayAccountReceivableTest : DesafioTargetSistemasClassFixture
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly string _token;

    public PayAccountReceivableTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _factory = factory;
        _token = factory.GetToken();
    }

    private static string Method(int id) => $"api/hml/accountsreceivable/{id}/payment";

    [Fact]
    public async Task Success()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var account = _factory.GetAccountsReceivable().Single(a => a.PaymentDate is null && a.DueDate < today);

        var daysOverdue = InterestCalculator.DaysOverdue(account.DueDate, today);
        var expectedPaidAmount = account.Amount + InterestCalculator.Calculate(account.Amount, daysOverdue);

        // Act
        var response = await DoPatch(Method(account.Id), _token);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        // Assert - Response
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        responseData.RootElement.GetProperty("paymentDate").GetString().ShouldBe(today.ToString("yyyy-MM-dd"));
        responseData.RootElement.GetProperty("paidAmount").GetDecimal().ShouldBe(expectedPaidAmount);

        // Assert - Database
        var saved = _factory.GetAccountReceivableFromDatabase(account.Id);
        saved.PaymentDate.ShouldBe(today);
        saved.PaidAmount.ShouldBe(expectedPaidAmount);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task ErrorNotFound(string culture)
    {
        // Act
        var response = await DoPatch(Method(999), _token, culture);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
        var expectedMessage = ResourceMessageException.ResourceManager
            .GetString(nameof(ResourceMessageException.ACCOUNT_RECEIVABLE_NOT_FOUND), new CultureInfo(culture));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        errors.ShouldHaveSingleItem();
        errors.ShouldContain(e => e.GetString()!.Equals(expectedMessage));
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task ErrorAlreadyPaid(string culture)
    {
        // Arrange
        var paidAccount = _factory.GetAccountsReceivable().Single(a => a.PaymentDate is not null);

        // Act
        var response = await DoPatch(Method(paidAccount.Id), _token, culture);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
        var expectedMessage = ResourceMessageException.ResourceManager
            .GetString(nameof(ResourceMessageException.ACCOUNT_RECEIVABLE_ALREADY_PAID), new CultureInfo(culture));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        errors.ShouldHaveSingleItem();
        errors.ShouldContain(e => e.GetString()!.Equals(expectedMessage));

        var saved = _factory.GetAccountReceivableFromDatabase(paidAccount.Id);
        saved.PaymentDate.ShouldBe(paidAccount.PaymentDate);
        saved.PaidAmount.ShouldBe(paidAccount.PaidAmount);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task ErrorWithoutToken(string culture)
    {
        // Arrange
        var account = _factory.GetAccountsReceivable().First(a => a.PaymentDate is null);

        // Act
        var response = await DoPatch(Method(account.Id), culture: culture);

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
        var account = _factory.GetAccountsReceivable().First(a => a.PaymentDate is null);

        // Act
        var response = await DoPatch(Method(account.Id), token: "tokenInvalido", culture: culture);

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
