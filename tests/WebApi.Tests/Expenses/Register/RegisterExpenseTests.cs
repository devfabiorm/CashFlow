using CashFlow.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Tests.InlineData;

namespace WebApi.Tests.Expenses.Register;
public class RegisterExpenseTests : CashFlowClassFixture
{
    private const string METHOD = "api/Expenses";

    private readonly string _token;

    public RegisterExpenseTests(CustomWebApplicationFactory webApplicationFactory) : base(webApplicationFactory)
    {
        _token = webApplicationFactory.User_Team_Member.GetToken();
    }

    [Fact]
    public async Task Success()
    {
        //Arrange
        var request = RequestRegisterExpenseJsonBuilder.Build();

        //Act
        var result = await DoPost(requestUri: METHOD, request: request, token: _token);

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);
        
        response.RootElement.GetProperty("title").GetString().Should().Be(request.Title);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Title_Empty(string culture)
    {
        //Arrange
        var request = RequestRegisterExpenseJsonBuilder.Build();
        request.Title = string.Empty;

        //Act
        var result = await DoPost(requestUri: METHOD, request: request, token: _token, culture: culture);

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var expectedLanguage = ResourceErrorMessages.ResourceManager.GetString("REQUIRED_TITLE", new CultureInfo(culture));

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.Should().HaveCount(1).And.Contain(error => error.GetString()!.Equals(expectedLanguage));
    }
}
