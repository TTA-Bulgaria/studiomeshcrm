using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Crm.Domain.Entities;
using Crm.Application.Interfaces;
using Crm.Application.DTOs.Auth;
using Crm.Application.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Crm.Application.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IGenericRepository<Tenant> _tenantRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;
    private readonly IEmailService _emailService;

    public AuthService(
        IUserRepository userRepository,
        IGenericRepository<Tenant> tenantRepository,
        IConfiguration configuration,
        ILogger<AuthService> logger,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
        _configuration = configuration;
        _logger = logger;
        _emailService = emailService;
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null) return null;

        var slug = System.Text.RegularExpressions.Regex.Replace(
            request.AgencyName.ToLowerInvariant().Trim(), @"[^a-z0-9]+", "-").Trim('-');
        var tenant = new Tenant { Name = request.AgencyName, Slug = slug };
        await _tenantRepository.AddAsync(tenant);

        var user = new User
        {
            Email = request.Email,
            FullName = request.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.Admin,
            TenantId = tenant.Id,
            HourlyRate = 0
        };

        await _userRepository.AddAsync(user);
        await SendVerificationEmailAsync(user);

        return new AuthResponse
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role.ToString(),
            TenantId = user.TenantId,
            TenantSlug = tenant.Slug,
            IsOnboardingCompleted = false
        };
    }

    public async Task<AuthResponse?> GetMeAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return null;

        return new AuthResponse
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role.ToString(),
            TenantId = user.TenantId,
            TenantSlug = user.Tenant?.Slug ?? string.Empty,
            IsOnboardingCompleted = user.Tenant?.OnboardingCompleted ?? false,
            AvatarUrl = user.AvatarUrl,
            JobTitle = user.JobTitle,
            PhoneNumber = user.PhoneNumber,
            HourlyRate = user.HourlyRate,
            BrandColor = user.Tenant?.BrandColor,
            LogoUrl = user.Tenant?.LogoUrl
        };
    }

    public async Task<(AuthResponse? Response, string? AccessToken, string? RefreshToken)> LoginAsync(LoginRequest request, string ipAddress)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return (null, null, null);

        if (!user.IsEmailVerified)
            throw new InvalidOperationException("EMAIL_NOT_VERIFIED");

        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken(ipAddress, user.Id);

        user.RefreshTokens.Add(refreshToken);
        await _userRepository.UpdateAsync(user);

        var response = new AuthResponse
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role.ToString(),
            TenantId = user.TenantId,
            TenantSlug = user.Tenant?.Slug ?? string.Empty,
            IsOnboardingCompleted = user.Tenant?.OnboardingCompleted ?? false,
            AvatarUrl = user.AvatarUrl,
            JobTitle = user.JobTitle,
            PhoneNumber = user.PhoneNumber,
            HourlyRate = user.HourlyRate,
            BrandColor = user.Tenant?.BrandColor,
            LogoUrl = user.Tenant?.LogoUrl
        };

        return (response, accessToken, refreshToken.Token);
    }

    public async Task<bool> VerifyEmailAsync(string token)
    {
        var user = await _userRepository.GetByEmailVerificationTokenAsync(token);
        if (user == null || user.IsEmailVerified) return false;

        user.IsEmailVerified = true;
        user.EmailVerificationToken = null;
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task ResendVerificationEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || user.IsEmailVerified) return;

        await SendVerificationEmailAsync(user);
    }

    private async Task SendVerificationEmailAsync(User user)
    {
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        user.EmailVerificationToken = token;
        await _userRepository.UpdateAsync(user);

        var appUrl = _configuration["FrontendUrl"] ?? _configuration["AppUrl"] ?? "https://app.studiomeshcrm.com";
        var verificationLink = $"{appUrl}/verify-email?token={token}";

        await _emailService.SendTemplatedEmailAsync(
            user.Email,
            "Verify your email address — Studio Mesh CRM",
            "EmailVerification",
            new EmailVerificationModel(user.FullName, verificationLink)
        );
    }

    public async Task ForgotPasswordAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null) return;

        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        user.PasswordResetToken = token;
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
        await _userRepository.UpdateAsync(user);

        var appUrl = _configuration["FrontendUrl"] ?? _configuration["AppUrl"] ?? "https://app.studiomeshcrm.com";
        var resetLink = $"{appUrl}/reset-password/{token}";

        await _emailService.SendTemplatedEmailAsync(
            user.Email,
            "Reset your password — Studio Mesh CRM",
            "PasswordReset",
            new PasswordResetModel(user.FullName, resetLink)
        );
    }

    public async Task<bool> ResetPasswordAsync(string token, string newPassword)
    {
        var user = await _userRepository.GetByResetTokenAsync(token);
        if (user == null || user.PasswordResetTokenExpiry < DateTime.UtcNow)
            return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;
        await _userRepository.UpdateAsync(user);

        try
        {
            await _emailService.SendTemplatedEmailAsync(
                user.Email,
                "Your password has been changed — Studio Mesh CRM",
                "PasswordResetConfirmation",
                new PasswordResetConfirmationModel(user.FullName, user.Email)
            );
        }
        catch
        {
            // Confirmation email failure must not roll back a successful password reset
        }

        return true;
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || !BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash)) return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<(AuthResponse? Response, string? AccessToken, string? RefreshToken)> RefreshTokenAsync(string token, string ipAddress)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(token);

        if (user == null) return (null, null, null);

        var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

        if (!refreshToken.IsActive) return (null, null, null);

        var newRefreshToken = RotateRefreshToken(refreshToken, ipAddress);
        user.RefreshTokens.Add(newRefreshToken);

        await _userRepository.UpdateAsync(user);

        var accessToken = GenerateJwtToken(user);

        return (new AuthResponse
        {
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role.ToString(),
            TenantId = user.TenantId,
            TenantSlug = user.Tenant?.Slug ?? string.Empty,
            IsOnboardingCompleted = user.Tenant?.OnboardingCompleted ?? false,
            AvatarUrl = user.AvatarUrl,
            JobTitle = user.JobTitle,
            BrandColor = user.Tenant?.BrandColor
        }, accessToken, newRefreshToken.Token);
    }

    public async Task<bool> RevokeTokenAsync(string token, string ipAddress)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(token);
        if (user == null) return false;

        var refreshToken = user.RefreshTokens.Single(x => x.Token == token);
        if (!refreshToken.IsActive) return false;

        refreshToken.Revoked = DateTime.UtcNow;
        refreshToken.RevokedByIp = ipAddress;

        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> CompleteOnboardingAsync(Guid userId, OnboardingRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        var tenant = await _tenantRepository.GetByIdAsync(user.TenantId);
        if (tenant == null) return false;

        user.JobTitle = request.JobTitle;
        user.PhoneNumber = request.PhoneNumber;
        user.AvatarUrl = request.LogoUrl;
        await _userRepository.UpdateAsync(user);

        tenant.Industry = request.Industry;
        tenant.CompanySize = request.CompanySize;
        tenant.Website = request.Website;
        tenant.TargetMonthlyRevenue = request.TargetMonthlyRevenue;
        tenant.BusinessAddress = request.BusinessAddress;
        tenant.BrandColor = request.BrandColor;
        tenant.LogoUrl = request.LogoUrl;
        tenant.OnboardingCompleted = true;
        await _tenantRepository.UpdateAsync(tenant);

        return true;
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is missing.")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim("sub", user.Id.ToString()),
            new Claim("email", user.Email),
            new Claim("tenant_id", user.TenantId.ToString()),
            new Claim("role", user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private RefreshToken GenerateRefreshToken(string ipAddress, Guid userId)
    {
        return new RefreshToken
        {
            Token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow,
            CreatedByIp = ipAddress,
            UserId = userId
        };
    }

    private RefreshToken RotateRefreshToken(RefreshToken refreshToken, string ipAddress)
    {
        var newRefreshToken = GenerateRefreshToken(ipAddress, refreshToken.UserId);
        refreshToken.Revoked = DateTime.UtcNow;
        refreshToken.RevokedByIp = ipAddress;
        refreshToken.ReplacedByToken = newRefreshToken.Token;
        return newRefreshToken;
    }
}
