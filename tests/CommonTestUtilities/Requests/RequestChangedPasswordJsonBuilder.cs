using Bogus;
using CashFlow.Communication.Requests;

namespace CommonTestUtilities.Requests;
public class RequestChangePasswordJsonBuilder
{
    public static RequestChangePasswordJson Build()
    {
        return new Faker<RequestChangePasswordJson>()
            .RuleFor(p => p.Password, faker => faker.Internet.Password())
            .RuleFor(p => p.NewPassword, faker => faker.Internet.Password(prefix: "!Aa1"));
    }
}
