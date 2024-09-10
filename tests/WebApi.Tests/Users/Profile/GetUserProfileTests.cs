using FluentAssertions;
using System.Net;
using System.Text.Json;

namespace WebApi.Tests.Users.Profile;
public class GetUserProfileTests : CashFlowClassFixture
{
    private const string METHOD = "api/Users";

    private readonly string _token;
    private readonly string _userName;
    private readonly string _userEmail;

    public GetUserProfileTests(CustomWebApplicationFactory webApplicationFactory) : base(webApplicationFactory)
    {
        _token = webApplicationFactory.User_Team_Member.GetToken();
        _userEmail = webApplicationFactory.User_Team_Member.GetEmail();
        _userName = webApplicationFactory.User_Team_Member.GetName();

    }

    [Fact]
    public async Task Success()
    {
        var result = await DoGet(requestUri: $"{METHOD}", token: _token);

        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        response.RootElement.GetProperty("name").GetString().Should().Be(_userName);
        response.RootElement.GetProperty("email").GetString().Should().Be(_userEmail);
    }
}
