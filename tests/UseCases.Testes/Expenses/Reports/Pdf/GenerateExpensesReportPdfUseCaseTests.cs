using CashFlow.Application.UseCases.Expenses.Reports.Pdf;
using CashFlow.Domain.Entities;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using FluentAssertions;

namespace UseCases.Testes.Expenses.Reports.Pdf;
public class GenerateExpensesReportPdfUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        //Arrange
        var loggedUser = UserBuilder.Build();
        var expenses = ExpenseBuilder.Collection(loggedUser);

        var useCase = CreateUseCase(loggedUser, expenses);

        //Act
        var result = await useCase.Execute(DateOnly.FromDateTime(DateTime.Today));

        //Assert
        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Success_Empty()
    {
        //Arrange
        var loggedUser = UserBuilder.Build();

        var useCase = CreateUseCase(loggedUser, []);

        //Act
        var result = await useCase.Execute(DateOnly.FromDateTime(DateTime.Today));

        //Assert
        result.Should().BeEmpty();
    }

    private GenerateExpensesReportPdfUseCase CreateUseCase(User user, List<Expense> expenses)
    {
        var repository = new ExpensesReadOnlyRepositoryBuilder()
            .FilterByMonth(user, expenses)
            .Build();

        var loggedUser = LoggedUserBuilder.Build(user);

        return new GenerateExpensesReportPdfUseCase(repository, loggedUser);
    }
}
