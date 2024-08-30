using FluentAssertions;
using System.Net;
using System.Text.Json;

namespace WebApi.Tests.Expenses.GetAll;
public class GetAllExpenseTests : CashFlowClassFixture
{
    private const string METHOD = "api/Expenses";

    private readonly string _token;
    public GetAllExpenseTests(CustomWebApplicationFactory webApplicationFactory) : base(webApplicationFactory)
    {
        _token = webApplicationFactory.GetToken();
    }

    [Fact]
    public async Task Success()
    {
        //Arrange
        //Act
        var result = await DoGet(requestUri: METHOD, token: _token);

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        response.RootElement.GetProperty("expenses").EnumerateArray().Should().NotBeNullOrEmpty();
    }
}
