using Moq;
using Crm.Application.Services;
using Crm.Application.Interfaces;
using Crm.Domain.Entities;
using Crm.Application.DTOs.Auth;
using Xunit;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Crm.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IGenericRepository<Tenant>> _tenantRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Fixture _fixture;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _tenantRepositoryMock = new Mock<IGenericRepository<Tenant>>();
        _configurationMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<AuthService>>();
        _emailServiceMock = new Mock<IEmailService>();
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        var jwtSectionMock = new Mock<IConfigurationSection>();
        jwtSectionMock.Setup(s => s["Key"]).Returns("super_secret_long_key_for_agency_crm_mvp_development_123!");
        jwtSectionMock.Setup(s => s["Issuer"]).Returns("agency_crm");
        jwtSectionMock.Setup(s => s["Audience"]).Returns("agency_crm");

        _configurationMock.Setup(c => c.GetSection("Jwt")).Returns(jwtSectionMock.Object);

        _service = new AuthService(
            _userRepositoryMock.Object,
            _tenantRepositoryMock.Object,
            _configurationMock.Object,
            _loggerMock.Object,
            _emailServiceMock.Object);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsAuthResponse()
    {
        var password = "Password123!";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            IsEmailVerified = true,
            Role = UserRole.Admin,
            TenantId = Guid.NewGuid(),
            RefreshTokens = new List<RefreshToken>(),
            Tenant = new Tenant { OnboardingCompleted = true }
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        var result = await _service.LoginAsync(new LoginRequest { Email = user.Email, Password = password }, "127.0.0.1");

        result.Response.Should().NotBeNull();
        result.Response!.Email.Should().Be(user.Email);
        result.Response.IsOnboardingCompleted.Should().BeTrue();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ReturnsNull()
    {
        var user = new User { Email = "test@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectOne") };
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        var result = await _service.LoginAsync(new LoginRequest { Email = user.Email, Password = "WrongPassword" }, "127.0.0.1");

        result.Response.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_UnverifiedEmail_ThrowsEmailNotVerifiedException()
    {
        var password = "Password123!";
        var user = new User
        {
            Email = "unverified@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            IsEmailVerified = false,
            RefreshTokens = new List<RefreshToken>()
        };
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        var act = () => _service.LoginAsync(new LoginRequest { Email = user.Email, Password = password }, "127.0.0.1");

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("EMAIL_NOT_VERIFIED");
    }

    [Fact]
    public async Task RegisterAsync_NewUser_CreatesTenantAndSendsVerificationEmail()
    {
        var request = new RegisterRequest
        {
            Email = "new@agency.com",
            FullName = "Jane Doe",
            AgencyName = "Doe Agency",
            Password = "Password123!"
        };
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync((User?)null);

        var result = await _service.RegisterAsync(request);

        result.Should().NotBeNull();
        result!.Email.Should().Be(request.Email);
        result.IsOnboardingCompleted.Should().BeFalse();
        _tenantRepositoryMock.Verify(r => r.AddAsync(It.Is<Tenant>(t => t.Name == request.AgencyName)), Times.Once);
        _userRepositoryMock.Verify(r => r.AddAsync(It.Is<User>(u => u.Email == request.Email)), Times.Once);
        _emailServiceMock.Verify(
            e => e.SendTemplatedEmailAsync(request.Email, It.Is<string>(s => s.Contains("Verify")), "EmailVerification", It.IsAny<object>()),
            Times.Once);
    }

    [Fact]
    public async Task VerifyEmailAsync_ValidToken_VerifiesUserAndClearsToken()
    {
        var token = "valid-token-abc123";
        var user = new User { IsEmailVerified = false, EmailVerificationToken = token };
        _userRepositoryMock.Setup(r => r.GetByEmailVerificationTokenAsync(token)).ReturnsAsync(user);

        var result = await _service.VerifyEmailAsync(token);

        result.Should().BeTrue();
        user.IsEmailVerified.Should().BeTrue();
        user.EmailVerificationToken.Should().BeNull();
        _userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task VerifyEmailAsync_InvalidToken_ReturnsFalse()
    {
        _userRepositoryMock.Setup(r => r.GetByEmailVerificationTokenAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var result = await _service.VerifyEmailAsync("bad-token");

        result.Should().BeFalse();
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task VerifyEmailAsync_AlreadyVerified_ReturnsFalse()
    {
        var token = "some-token";
        var user = new User { IsEmailVerified = true, EmailVerificationToken = token };
        _userRepositoryMock.Setup(r => r.GetByEmailVerificationTokenAsync(token)).ReturnsAsync(user);

        var result = await _service.VerifyEmailAsync(token);

        result.Should().BeFalse();
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task ResendVerificationEmailAsync_UnverifiedUser_SendsEmail()
    {
        var user = new User { Email = "unverified@example.com", FullName = "Test User", IsEmailVerified = false };
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        await _service.ResendVerificationEmailAsync(user.Email);

        _emailServiceMock.Verify(
            e => e.SendTemplatedEmailAsync(user.Email, It.IsAny<string>(), "EmailVerification", It.IsAny<object>()),
            Times.Once);
    }

    [Fact]
    public async Task ResendVerificationEmailAsync_AlreadyVerified_DoesNotSendEmail()
    {
        var user = new User { Email = "verified@example.com", IsEmailVerified = true };
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        await _service.ResendVerificationEmailAsync(user.Email);

        _emailServiceMock.Verify(
            e => e.SendTemplatedEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()),
            Times.Never);
    }

    [Fact]
    public async Task ForgotPasswordAsync_ValidEmail_SendsResetEmail()
    {
        var user = new User { Email = "user@example.com", FullName = "Test User" };
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        await _service.ForgotPasswordAsync(user.Email);

        user.PasswordResetToken.Should().NotBeNullOrEmpty();
        user.PasswordResetTokenExpiry.Should().BeAfter(DateTime.UtcNow);
        _emailServiceMock.Verify(
            e => e.SendTemplatedEmailAsync(user.Email, It.Is<string>(s => s.Contains("Reset")), "PasswordReset", It.IsAny<object>()),
            Times.Once);
    }

    [Fact]
    public async Task ForgotPasswordAsync_UnknownEmail_DoesNothing()
    {
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        await _service.ForgotPasswordAsync("nobody@example.com");

        _emailServiceMock.Verify(
            e => e.SendTemplatedEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()),
            Times.Never);
    }

    [Fact]
    public async Task ResetPasswordAsync_ValidToken_UpdatesPasswordAndSendsConfirmation()
    {
        var token = "valid-reset-token";
        var user = new User
        {
            Email = "user@example.com",
            FullName = "Test User",
            PasswordResetToken = token,
            PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1)
        };
        _userRepositoryMock.Setup(r => r.GetByResetTokenAsync(token)).ReturnsAsync(user);

        var result = await _service.ResetPasswordAsync(token, "NewPassword123!");

        result.Should().BeTrue();
        user.PasswordResetToken.Should().BeNull();
        BCrypt.Net.BCrypt.Verify("NewPassword123!", user.PasswordHash).Should().BeTrue();
        _emailServiceMock.Verify(
            e => e.SendTemplatedEmailAsync(user.Email, It.IsAny<string>(), "PasswordResetConfirmation", It.IsAny<object>()),
            Times.Once);
    }

    [Fact]
    public async Task ResetPasswordAsync_ExpiredToken_ReturnsFalse()
    {
        var token = "expired-token";
        var user = new User
        {
            PasswordResetToken = token,
            PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(-1)
        };
        _userRepositoryMock.Setup(r => r.GetByResetTokenAsync(token)).ReturnsAsync(user);

        var result = await _service.ResetPasswordAsync(token, "NewPassword123!");

        result.Should().BeFalse();
        _emailServiceMock.Verify(
            e => e.SendTemplatedEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()),
            Times.Never);
    }

    [Fact]
    public async Task ResetPasswordAsync_InvalidToken_ReturnsFalse()
    {
        _userRepositoryMock.Setup(r => r.GetByResetTokenAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var result = await _service.ResetPasswordAsync("bad-token", "NewPassword123!");

        result.Should().BeFalse();
    }

    [Fact]
    public async Task RefreshTokenAsync_ValidToken_RotatesToken()
    {
        var oldToken = "old-token";
        var user = _fixture.Create<User>();
        var refreshToken = new RefreshToken { Token = oldToken, Expires = DateTime.UtcNow.AddDays(1), Created = DateTime.UtcNow, UserId = user.Id };
        user.RefreshTokens = new List<RefreshToken> { refreshToken };

        _userRepositoryMock.Setup(r => r.GetByRefreshTokenAsync(oldToken)).ReturnsAsync(user);

        var result = await _service.RefreshTokenAsync(oldToken, "127.0.0.1");

        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBe(oldToken);
        refreshToken.Revoked.Should().NotBeNull();
        _userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task CompleteOnboardingAsync_ValidData_UpdatesUserAndTenant()
    {
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var user = new User { Id = userId, TenantId = tenantId };
        var tenant = new Tenant { Id = tenantId, OnboardingCompleted = false };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _tenantRepositoryMock.Setup(r => r.GetByIdAsync(tenantId)).ReturnsAsync(tenant);

        var request = new OnboardingRequest
        {
            JobTitle = "CEO",
            Industry = "SaaS",
            TargetMonthlyRevenue = 50000,
            BrandColor = "#000000"
        };

        var success = await _service.CompleteOnboardingAsync(userId, request);

        success.Should().BeTrue();
        user.JobTitle.Should().Be(request.JobTitle);
        tenant.Industry.Should().Be(request.Industry);
        tenant.OnboardingCompleted.Should().BeTrue();
        _userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        _tenantRepositoryMock.Verify(r => r.UpdateAsync(tenant), Times.Once);
    }
}
