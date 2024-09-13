using CashFlow.Application.UseCases.Users.Delete;
using CashFlow.Domain.Entities;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using FluentAssertions;

namespace UseCases.Testes.Users.Delete;
public class DeleteUserAccountUseCaseTests
{
    public async Task Success()
    {
        //Arrange
        var user = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        //Act
        var act = async () => await useCase.Execute();

        //Assert
        await act.Should().NotThrowAsync();
    }

    private DeleteUserAccountUseCase CreateUseCase(User user)
    {
        var loggedUser = LoggedUserBuilder.Build(user);
        var repository = UserWriteOnlyRepositoryBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();

        return new DeleteUserAccountUseCase(loggedUser, repository, unitOfWork);
    }
}
