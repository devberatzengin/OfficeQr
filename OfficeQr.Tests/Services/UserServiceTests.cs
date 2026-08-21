using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using OfficeQr.Entity;
using OfficeQr.Exceptions;
using OfficeQr.Services;
using OfficeQr.Services.Interfaces;

namespace OfficeQr.Tests.Services;

public class UserServiceTests
{
    private static UserManager<User> CreateUserManagerMock()
    {
        // UserManager<User>'ın kendisi Moq ile mock'lanabilir çünkü metotları virtual;
        // constructor sadece IUserStore ister, geri kalan bağımlılıklara bu testte
        // hiç ihtiyacımız olmadığı için null geçiyoruz.
        var storeMock = new Mock<IUserStore<User>>();
        var userManagerMock = new Mock<UserManager<User>>(
            storeMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        return userManagerMock.Object;
    }

    [Fact]
    public async Task DeleteAsync_WhenAdminTargetsOwnAccount_ThrowsBadRequestException()
    {
        // Arrange
        var adminId = Guid.NewGuid();

        var currentUserServiceMock = new Mock<ICurrentUserService>();
        currentUserServiceMock.Setup(s => s.GetCurrentUserId()).Returns(adminId);

        var mapperMock = new Mock<IMapper>();
        var userManager = CreateUserManagerMock();

        var sut = new UserService(userManager, mapperMock.Object, currentUserServiceMock.Object);

        // Act
        var act = async () => await sut.DeleteAsync(adminId, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<BadRequestException>(act);

        // Kendi hesabına işlem engeli en başta devreye girdiği için
        // UserManager'a hiç gidilmediğini de doğruluyoruz.
        Mock.Get(userManager).Verify(
            m => m.FindByIdAsync(It.IsAny<string>()),
            Times.Never);
    }
}
