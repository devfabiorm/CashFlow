using CashFlow.Domain.Security.Cryptography;
using Moq;

namespace CommonTestUtilities.Cryptography;
public class PasswordEncrypterBuilder
{
    public static IPasswordEncrypter Build()
    {
        var mock = new Mock<IPasswordEncrypter>();

        mock.Setup(passwordEncryptyer => passwordEncryptyer.Encrypt(It.IsAny<string>()))
            .Returns("!a$%*adj@!d23");

        return mock.Object;
    }
}
