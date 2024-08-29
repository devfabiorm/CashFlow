using CashFlow.Communication.Requests;
using CashFlow.Exception;
using CommonTestUtilities.Requests;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentAssertions;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Tests.InlineData;

namespace WebApi.Tests.Login.DoLogin;
public class DoLoginTest : CashFlowClassFixture
{
    private const string METHOD = "api/Login";

    private readonly string _email;
    private readonly string _name;
    private readonly string _password;

    public DoLoginTest(CustomWebApplicationFactory webApplicationFactory) : base(webApplicationFactory)
    {
        _email = webApplicationFactory.GetEmail();
        _name = webApplicationFactory.GetName();
        _password = webApplicationFactory.GetPassword();
    }

    [Fact]
    public async Task Success()
    {
        //Arrange
        var request = new RequestLoginJson
        {
            Email = _email,
            Password = _password
        };

        //Act
        var response = await DoPost(METHOD, request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("name").GetString().Should().Be(_name);
        responseData.RootElement.GetProperty("token").GetString().Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Login_Invalid(string culture)
    {
        //Arrange
        var request = RequestLoginJsonBuilder.Build();

        //Act
        var response = await DoPost(requestUri: METHOD, request: request, culture: culture);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var error = responseData.RootElement.GetProperty("errorMessages").EnumerateArray();

        var expectedMessage = ResourceErrorMessages.ResourceManager.GetString("EMAIL_OR_PASSWORD_INVALID", new CultureInfo(culture));

        error.Should().HaveCount(1).And.Contain(c => c.GetString()!.Equals(expectedMessage));
    }
}
