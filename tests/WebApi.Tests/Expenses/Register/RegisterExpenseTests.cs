using CashFlow.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Tests.InlineData;

namespace WebApi.Tests.Expenses.Register;
public class RegisterExpenseTests : IClassFixture<CustomWebApplicationFactory>
{
    private const string METHOD = "api/Expenses";

    private readonly HttpClient _httpClient;
    private readonly string _token;

    public RegisterExpenseTests(CustomWebApplicationFactory webApplicationFactory)
    {
        _httpClient = webApplicationFactory.CreateClient();
        _token = webApplicationFactory.GetToken();
    }

    [Fact]
    public async Task Success()
    {
        //Arrange
        var request = RequestRegisterExpenseJsonBuilder.Build();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

        //Act
        var result = await _httpClient.PostAsJsonAsync(METHOD, request);

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);
        
        response.RootElement.GetProperty("title").GetString().Should().Be(request.Title);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Title_Empty(string cultureInfo)
    {
        //Arrange
        var request = RequestRegisterExpenseJsonBuilder.Build();
        request.Title = string.Empty;

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        _httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(cultureInfo));

        //Act
        var result = await _httpClient.PostAsJsonAsync(METHOD, request);

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var expectedLanguage = ResourceErrorMessages.ResourceManager.GetString("REQUIRED_TITLE", new CultureInfo(cultureInfo));

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.Should().HaveCount(1).And.Contain(error => error.GetString()!.Equals(expectedLanguage));
    }
}
