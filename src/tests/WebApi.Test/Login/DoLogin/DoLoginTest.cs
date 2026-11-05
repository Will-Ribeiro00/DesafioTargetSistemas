using CommonTestUtilities.Requests;
using DesafioTargetSistemas.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.Login.DoLogin;

public class DoLoginTest : DesafioTargetSistemasClassFixture
{
    private const string METHOD = "api/hml/login";

    private readonly string _email;
    private readonly string _password;

    public DoLoginTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _email = factory.GetUserEmail();
        _password = factory.GetUserPassword();
    }

    [Fact]
    public async Task Success()
    {
        // Arrange
        var request = RequestLoginJsonBuilder.Build();
        request.Email = _email;
        request.Password = _password;

        // Act
        var response = await DoPost(METHOD, request);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var token = responseData.RootElement.GetProperty("accessToken").GetString();

        // Assert - Response
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        token.ShouldNotBeNullOrWhiteSpace();

        // Assert - Protected Endpoint
        var protectedResponse = await DoGet("api/hml/products", token!);
        protectedResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task ErrorUserNotFound(string culture)
    {
        // Arrange
        var request = RequestLoginJsonBuilder.Build();

        // Act
        var response = await DoPost(METHOD, request, culture: culture);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
        var expectedMessage = ResourceMessageException.ResourceManager
            .GetString(nameof(ResourceMessageException.INVALID_LOGIN), new CultureInfo(culture));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        errors.ShouldHaveSingleItem();
        errors.ShouldContain(e => e.GetString()!.Equals(expectedMessage));
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task ErrorWrongPassword(string culture)
    {
        // Arrange
        var request = RequestLoginJsonBuilder.Build();
        request.Email = _email;
        request.Password = "SenhaErrada@123";

        // Act
        var response = await DoPost(METHOD, request, culture: culture);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
        var expectedMessage = ResourceMessageException.ResourceManager
            .GetString(nameof(ResourceMessageException.INVALID_LOGIN), new CultureInfo(culture));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        errors.ShouldHaveSingleItem();
        errors.ShouldContain(e => e.GetString()!.Equals(expectedMessage));
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task ErrorInvalidRequest(string culture)
    {
        // Arrange
        var request = RequestLoginJsonBuilder.Build();
        request.Email = string.Empty;

        // Act
        var response = await DoPost(METHOD, request, culture: culture);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
        var expectedMessage = ResourceMessageException.ResourceManager
            .GetString(nameof(ResourceMessageException.EMAIL_REQUIRED), new CultureInfo(culture));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        errors.ShouldHaveSingleItem();
        errors.ShouldContain(e => e.GetString()!.Equals(expectedMessage));
    }
}
