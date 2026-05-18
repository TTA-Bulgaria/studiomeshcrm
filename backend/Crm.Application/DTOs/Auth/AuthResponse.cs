namespace Crm.Application.DTOs.Auth;

public class AuthResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public string TenantSlug { get; set; } = string.Empty;
    public bool IsOnboardingCompleted { get; set; }
    public string AccessToken { get; set; } = string.Empty;

    // Profile fields
    public string? AvatarUrl { get; set; }
    public string? JobTitle { get; set; }
    public string? PhoneNumber { get; set; }
    public decimal HourlyRate { get; set; }

    // Branding fields
    public string? BrandColor { get; set; }
    public string? LogoUrl { get; set; }
}
