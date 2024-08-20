using CommonTestUtilities.Requests;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace WebApi.Tests.Users.Register;
public class RegisterUserTests : IClassFixture<CustomWebApplicationFactory>
{
    private const string METHOD = "api/User";

    private readonly HttpClient _httpClient;

    public RegisterUserTests(CustomWebApplicationFactory webApplicationFactory)
    {
        _httpClient = webApplicationFactory.CreateClient();
    }

    [Fact]
    public async Task Success()
    {
        //Arrange
        var request = RequestRegisterExpenseJsonBuilder.Build();

        //Act
        var result = await _httpClient.PostAsJsonAsync(METHOD, request);

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
