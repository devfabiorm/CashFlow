using CashFlow.Application.UseCases.Users.ChangePassword;
using CashFlow.Domain.Entities;
using CashFlow.Exception;
using CashFlow.Exception.ExceptionsBase;
using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using FluentAssertions;

namespace UseCases.Testes.Users.ChangePassword;
public class ChangePasswordUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        //Arrange
        var request = RequestChangePasswordJsonBuilder.Build();
        var user = UserBuilder.Build();

        var useCase = CreateUseCase(user, request.Password);

        //Act
        var act = async () => await useCase.Execute(request);

        //Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Error_NewPassword_Empty()
    {
        //Arrange
        var request = RequestChangePasswordJsonBuilder.Build();
        request.NewPassword = string.Empty;

        var user = UserBuilder.Build();

        var useCase = CreateUseCase(user, request.Password);

        //Act
        var act = async () => await useCase.Execute(request);

        //Assert
        var exception = await act.Should().ThrowAsync<ErrorOnValidationException>();
        exception.Where(ex => ex.GetErrorMessages().Count == 1 && ex.GetErrorMessages().Contains(ResourceErrorMessages.INVALID_PASSWORD));
    }

    [Fact]
    public async Task Error_Current_Password_Different()
    {
        //Arrange
        var request = RequestChangePasswordJsonBuilder.Build();

        var user = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        //Act
        var act = async () => await useCase.Execute(request);

        //Assert
        var exception = await act.Should().ThrowAsync<ErrorOnValidationException>();
        exception.Where(ex => ex.GetErrorMessages().Count == 1 && ex.GetErrorMessages().Contains(ResourceErrorMessages.PASSWORD_DIFFERENT_CURRENT_PASSWORD));
    }

    private ChangePasswordUseCase CreateUseCase(User user, string? password = null)
    {
        var loggedUser = LoggedUserBuilder.Build(user);
        var updateRepository = UserUpdateOnlyRepositoryBuilder.Build(user);
        var unitOfWork = UnitOfWorkBuilder.Build();
        var passwordEncrypter = new PasswordEncrypterBuilder().Verify(password).Build();


        return new ChangePasswordUseCase(loggedUser, updateRepository, unitOfWork, passwordEncrypter);
    }
}
