using CashFlow.Domain.Entities;
using CashFlow.Domain.Security.Cryptography;
using CashFlow.Domain.Security.Tokens;
using CashFlow.Infrastructure.DataAccess;
using CommonTestUtilities.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Tests.Resources;

namespace WebApi.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public ExpenseIdentityManager Expense { get; private set; } = default!;
    public UserIdentityManager User_Team_Member { get; private set; } = default!;
    public UserIdentityManager User_Admin { get; private set; } = default!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test")
            .ConfigureServices(services =>
            {
                var provider = services.AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

                services.AddDbContext<CashFlowDbContext>(config =>
                {
                    config.UseInMemoryDatabase("InMemoryDbForTesting");
                    config.UseInternalServiceProvider(provider);
                });

                var scope = services.BuildServiceProvider().CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<CashFlowDbContext>();
                var passwordEncrypter = scope.ServiceProvider.GetRequiredService<IPasswordEncrypter>();
                var tokenGenerator = scope.ServiceProvider.GetRequiredService<IAccessTokenGenerator>();

                StartDatabase(dbContext, passwordEncrypter, tokenGenerator);

                
            });
    }

    private void StartDatabase(CashFlowDbContext dbContext, IPasswordEncrypter passwordEncrypter, IAccessTokenGenerator accessTokenGenerator)
    {
        var user = AddUserTeamMember(dbContext, passwordEncrypter, accessTokenGenerator);
        AddExpenses(dbContext, user);
        dbContext.SaveChanges();
    }

    private User AddUserTeamMember(CashFlowDbContext dbContext, IPasswordEncrypter passwordEncrypter, IAccessTokenGenerator accessTokenGenerator)
    {
        var user = UserBuilder.Build();
        var password = user.Password;

        user.Password = passwordEncrypter.Encrypt(user.Password);

        dbContext.Users.Add(user);
        
        var token = accessTokenGenerator.Generate(user);
        User_Team_Member = new UserIdentityManager(user, password, token);

        return user;
    }

    private void AddExpenses(CashFlowDbContext dbContext, User user) 
    {
        var expenses = ExpenseBuilder.Collection(user);

        dbContext.Expenses.AddRange(expenses);
        Expense = new ExpenseIdentityManager(expenses[0]);
    }
}
