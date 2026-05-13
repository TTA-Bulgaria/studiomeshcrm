using Crm.Application.DTOs.Auth;
using Crm.Application.Services;
using Crm.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Crm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<AuthController> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly ICurrentUserContext _userContext;

    public AuthController(AuthService authService, ILogger<AuthController> logger, IWebHostEnvironment env, ICurrentUserContext userContext)
    {
        _authService = authService;
        _logger = logger;
        _env = env;
        _userContext = userContext;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt for user: {Email}", request.Email);
        (AuthResponse? response, string? accessToken, string? refreshToken) result;
        try
        {
            result = await _authService.LoginAsync(request, GetIpAddress());
        }
        catch (InvalidOperationException ex) when (ex.Message == "EMAIL_NOT_VERIFIED")
        {
            return Unauthorized(new { Message = "Please verify your email before signing in. Check your inbox for the verification link.", Code = "EMAIL_NOT_VERIFIED" });
        }

        var (response, accessToken, refreshToken) = result;
        if (response == null)
        {
            _logger.LogWarning("Failed login attempt for user: {Email}", request.Email);
            return Unauthorized(new { Message = "Invalid email or password." });
        }

        _logger.LogInformation("User {Email} logged in successfully", request.Email);
        SetTokenCookie("refresh_token", refreshToken!);
        SetTokenCookie("access_token", accessToken!);

        response.AccessToken = accessToken!;
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        _logger.LogInformation("Registration attempt for email: {Email}, Agency: {Agency}", request.Email, request.AgencyName);
        var response = await _authService.RegisterAsync(request);

        if (response == null)
        {
            return BadRequest(new { Message = "User with this email already exists." });
        }

        return Ok(new { Message = "Registration successful. Please check your email to verify your account before signing in." });
    }

    [AllowAnonymous]
    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        var success = await _authService.VerifyEmailAsync(token);
        if (!success) return NotFound(new { Message = "Invalid or already used verification link." });
        return Ok(new { Message = "Email verified successfully. You can now sign in." });
    }

    [AllowAnonymous]
    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification([FromBody] ForgotPasswordRequest request)
    {
        await _authService.ResendVerificationEmailAsync(request.Email);
        return Ok(new { Message = "If that email is registered and unverified, a new link has been sent." });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(refreshToken)) return BadRequest("Token is required.");

        var (response, accessToken, newRefreshToken) = await _authService.RefreshTokenAsync(refreshToken, GetIpAddress());

        if (response == null) return Unauthorized("Invalid token.");

        SetTokenCookie("refresh_token", newRefreshToken!);
        SetTokenCookie("access_token", accessToken!);

        response.AccessToken = accessToken!;
        return Ok(response);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refresh_token"];
        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _authService.RevokeTokenAsync(refreshToken, GetIpAddress());
        }

        var deleteOptions = new CookieOptions
        {
            Domain = _env.IsDevelopment() ? null : ".studiomeshcrm.com",
            Path = "/",
            Secure = !_env.IsDevelopment(),
            SameSite = _env.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.None,
        };
        Response.Cookies.Delete("access_token", deleteOptions);
        Response.Cookies.Delete("refresh_token", deleteOptions);

        return Ok(new { Message = "Logged out successfully." });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = _userContext.UserId;
        if (!userId.HasValue) return Unauthorized();

        var response = await _authService.GetMeAsync(userId.Value);
        if (response == null) return Unauthorized();

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        await _authService.ForgotPasswordAsync(request.Email);
        return Ok(new { Message = "If an account exists with that email, a reset link has been sent." });
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var success = await _authService.ResetPasswordAsync(request.Token, request.NewPassword);
        if (!success) return BadRequest(new { Message = "Invalid or expired token." });
        return Ok(new { Message = "Password has been reset successfully." });
    }

    [Authorize]
    [HttpPost("onboarding/complete")]
    public async Task<IActionResult> CompleteOnboarding([FromBody] OnboardingRequest request)
    {
        var userId = _userContext.UserId;
        if (!userId.HasValue) return Unauthorized();

        var success = await _authService.CompleteOnboardingAsync(userId.Value, request);
        if (!success) return NotFound(new { Message = "User not found." });

        return Ok(new { Message = "Onboarding completed successfully." });
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = _userContext.UserId;
        if (!userId.HasValue) return Unauthorized();

        var success = await _authService.ChangePasswordAsync(userId.Value, request.CurrentPassword, request.NewPassword);
        if (!success) return BadRequest(new { Message = "Current password is incorrect." });

        return Ok(new { Message = "Password changed successfully." });
    }

    private void SetTokenCookie(string name, string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = name == "refresh_token" ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddMinutes(15),
            Secure = !_env.IsDevelopment(),
            SameSite = _env.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.None,
            Domain = _env.IsDevelopment() ? null : ".studiomeshcrm.com",
            Path = "/"
        };
        Response.Cookies.Append(name, token, cookieOptions);
    }

    private string GetIpAddress()
    {
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
            return Request.Headers["X-Forwarded-For"]!;
        else
            return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "N/A";
    }
}
