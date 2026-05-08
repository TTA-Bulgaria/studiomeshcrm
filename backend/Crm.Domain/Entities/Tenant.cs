namespace Crm.Domain.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? BillingEmail { get; set; }
    public string Currency { get; set; } = "USD";
    public string? TaxId { get; set; }
    public int DefaultPaymentTermsDays { get; set; } = 30;
    public string? Timezone { get; set; }
    public string? LogoUrl { get; set; }
    public string? BrandColor { get; set; }
    public string? Industry { get; set; }
    public string? CompanySize { get; set; }
    public string? Website { get; set; }
    public string? BusinessAddress { get; set; }
    public decimal? TargetMonthlyRevenue { get; set; }
    public bool OnboardingCompleted { get; set; } = false;

    public ICollection<User> Users { get; set; } = new List<User>();
}
