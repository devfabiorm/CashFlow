using FluentAssertions;
using System.Net;

namespace WebApi.Tests.Users.Delete;
public class DeleteUserAccountTests : CashFlowClassFixture
{
    private const string METHOD = "api/Users";

    private readonly string _token;

    public DeleteUserAccountTests(CustomWebApplicationFactory webApplicationFactory) : base(webApplicationFactory)
    {
        _token = webApplicationFactory.User_Team_Member.GetToken();
    }

    [Fact]
    public async Task Success()
    {
        var response = await DoDelete(requestUri: METHOD, token: _token);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
