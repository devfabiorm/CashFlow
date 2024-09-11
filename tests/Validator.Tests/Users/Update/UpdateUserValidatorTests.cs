using CashFlow.Application.UseCases.Users.Update;
using CashFlow.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;
using FluentValidation;

namespace Validator.Tests.Users.Update;
public class UpdateUserValidatorTests
{
    [Fact]
    public void Success()
    {
        //Arrange
        var request = RequestUpdateUserJsonBuilder.Build();
        var validator = new UpdateUserValidator();

        //Act
        var result = validator.Validate(request);

        //Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData(null)]
    public void Error_Name_Empty(string name)
    {
        //Arrange
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Name = name;

        var validator = new UpdateUserValidator();

        //Act
        var result = validator.Validate(request);

        //Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And.Contain(error => error.ErrorMessage.Equals(ResourceErrorMessages.NAME_EMPTY));
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData(null)]
    public void Error_Email_Empty(string email)
    {
        //Arrange
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Email = email;

        var validator = new UpdateUserValidator();

        //Act
        var result = validator.Validate(request);

        //Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And.Contain(error => error.ErrorMessage.Equals(ResourceErrorMessages.EMAIL_EMPTY));
    }

    [Fact]
    public void Error_Email_Invalid()
    {
        //Arrange
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Email = "test.com";

        var validator = new UpdateUserValidator();

        //Act
        var result = validator.Validate(request);

        //Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And.Contain(error => error.ErrorMessage.Equals(ResourceErrorMessages.EMAIL_INVALID));
    }
}
