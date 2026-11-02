using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using NetCoreIdentity.Models;
using NetCoreIdentity.Services;
using Xunit;

namespace NetCoreIdentity.Tests
{
    public class AccountServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManager;
        private readonly AccountService _sut;

        public AccountServiceTests()
        {
            _userManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null!, null!, null!, null!, null!, null!, null!, null!);

            _signInManager = new Mock<SignInManager<ApplicationUser>>(
                _userManager.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null!, null!, null!, null!);

            _sut = new AccountService(_userManager.Object, _signInManager.Object);
        }

        private static ApplicationUser MakeUser(bool isActive) =>
            new ApplicationUser { UserName = "a@test.com", Email = "a@test.com", IsActive = isActive };

        [Fact]
        public async Task Login_UnknownEmail_ReturnsInvalidCredentials()
        {
            _userManager.Setup(m => m.FindByEmailAsync("x@test.com"))
                        .ReturnsAsync((ApplicationUser?)null);

            var result = await _sut.LoginAsync("x@test.com", "Pass@123", false);

            Assert.Equal(LoginResult.InvalidCredentials, result);
        }

        [Fact]
        public async Task Login_DeactivatedUser_ReturnsDeactivated_AndNeverChecksPassword()
        {
            _userManager.Setup(m => m.FindByEmailAsync("a@test.com"))
                        .ReturnsAsync(MakeUser(isActive: false));

            var result = await _sut.LoginAsync("a@test.com", "Pass@123", false);

            Assert.Equal(LoginResult.Deactivated, result);
            _signInManager.Verify(s => s.PasswordSignInAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()),
                Times.Never);
        }

        [Fact]
        public async Task Login_ActiveUserCorrectPassword_ReturnsSuccess()
        {
            _userManager.Setup(m => m.FindByEmailAsync("a@test.com"))
                        .ReturnsAsync(MakeUser(isActive: true));
            _signInManager.Setup(s => s.PasswordSignInAsync("a@test.com", "Pass@123", false, true))
                          .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var result = await _sut.LoginAsync("a@test.com", "Pass@123", false);

            Assert.Equal(LoginResult.Success, result);
        }

        [Fact]
        public async Task Login_WrongPassword_ReturnsInvalidCredentials()
        {
            _userManager.Setup(m => m.FindByEmailAsync("a@test.com"))
                        .ReturnsAsync(MakeUser(isActive: true));
            _signInManager.Setup(s => s.PasswordSignInAsync("a@test.com", "wrong", false, true))
                          .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var result = await _sut.LoginAsync("a@test.com", "wrong", false);

            Assert.Equal(LoginResult.InvalidCredentials, result);
        }

        [Fact]
        public async Task Login_LockedOutUser_ReturnsLockedOut()
        {
            _userManager.Setup(m => m.FindByEmailAsync("a@test.com"))
                        .ReturnsAsync(MakeUser(isActive: true));
            _signInManager.Setup(s => s.PasswordSignInAsync("a@test.com", "Pass@123", false, true))
                          .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.LockedOut);

            var result = await _sut.LoginAsync("a@test.com", "Pass@123", false);

            Assert.Equal(LoginResult.LockedOut, result);
        }
    }
}