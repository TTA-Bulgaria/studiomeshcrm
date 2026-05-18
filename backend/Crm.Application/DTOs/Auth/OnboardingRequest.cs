namespace Crm.Application.DTOs.Auth;

public class OnboardingRequest
{
    public string? JobTitle { get; set; }
    public string? PhoneNumber { get; set; }
    public string? LogoUrl { get; set; }
    public string? Industry { get; set; }
    public string? CompanySize { get; set; }
    public string? Website { get; set; }
    public string? BusinessAddress { get; set; }
    public string? BrandColor { get; set; }
    public decimal? TargetMonthlyRevenue { get; set; }
}
