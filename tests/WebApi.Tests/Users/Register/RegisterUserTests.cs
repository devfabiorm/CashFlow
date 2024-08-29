using CashFlow.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Tests.InlineData;

namespace WebApi.Tests.Users.Register;
public class RegisterUserTests : CashFlowClassFixture
{
    private const string METHOD = "api/Users";

    public RegisterUserTests(CustomWebApplicationFactory webApplicationFactory) : base(webApplicationFactory)
    {
    }

    [Fact]
    public async Task Success()
    {
        //Arrange
        var request = RequestRegisterUserJsonBuilder.Build();

        //Act
        var result = await DoPost(METHOD, request);

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        response.RootElement.GetProperty("name").GetString().Should().Be(request.Name);
        response.RootElement.GetProperty("token").GetString().Should().NotBeNullOrEmpty();
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Empty_Name(string culture)
    {
        //Arrange
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Name = string.Empty;

        //Act
        var result = await DoPost(requestUri: METHOD, request: request, culture: culture);

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        var expectedMessage = ResourceErrorMessages.ResourceManager.GetString("NAME_EMPTY", new CultureInfo(culture));

        errors.Should().HaveCount(1).And.Contain(error => error.GetString()!.Equals(expectedMessage));
    }
}
