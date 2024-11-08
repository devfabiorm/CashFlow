﻿using CashFlow.Domain.Entities;
using CashFlow.Domain.Enums;
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
    public ExpenseIdentityManager Expense_Admin { get; private set; } = default!;
    public ExpenseIdentityManager Expense_MemberTeam { get; private set; } = default!;
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
        var userTeamMember = AddUserTeamMember(dbContext, passwordEncrypter, accessTokenGenerator);
        var expenseTeamMember = AddExpenses(dbContext, userTeamMember, expenseId: 1, tagId: 1);
        Expense_MemberTeam = new ExpenseIdentityManager(expenseTeamMember);

        var useAdmin = AddUserAdmin(dbContext, passwordEncrypter, accessTokenGenerator);
        var expenseAdmin = AddExpenses(dbContext, useAdmin, expenseId: 3, beginFrom: 2, tagId: 2);
        Expense_Admin = new ExpenseIdentityManager(expenseAdmin);

        dbContext.SaveChanges();
    }

    private User AddUserTeamMember(CashFlowDbContext dbContext, IPasswordEncrypter passwordEncrypter, IAccessTokenGenerator accessTokenGenerator)
    {
        var user = UserBuilder.Build();
        user.Id = 1;

        var password = user.Password;

        user.Password = passwordEncrypter.Encrypt(user.Password);

        dbContext.Users.Add(user);
        
        var token = accessTokenGenerator.Generate(user);
        User_Team_Member = new UserIdentityManager(user, password, token);

        return user;
    }

    private User AddUserAdmin(CashFlowDbContext dbContext, IPasswordEncrypter passwordEncrypter, IAccessTokenGenerator accessTokenGenerator)
    {
        var user = UserBuilder.Build(Roles.ADMIN);
        user.Id = 2;

        var password = user.Password;

        user.Password = passwordEncrypter.Encrypt(user.Password);

        dbContext.Users.Add(user);

        var token = accessTokenGenerator.Generate(user);
        User_Admin = new UserIdentityManager(user, password, token);

        return user;
    }

    private Expense AddExpenses(CashFlowDbContext dbContext, User user, long expenseId, long tagId, uint beginFrom = 0) 
    {
        var expenses = ExpenseBuilder.Collection(user, beginFrom: beginFrom);
        expenses[0].Id = expenseId;
        var radom = new Random(3);

        foreach (var tag in expenses.SelectMany(e => e.Tags))
        {
            tag.Id = tagId * radom.NextInt64(600);
            tag.ExpenseId = expenseId;
        }

        dbContext.Expenses.AddRange(expenses);
        return expenses[0];
    }
}
