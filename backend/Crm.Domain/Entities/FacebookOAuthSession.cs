namespace Crm.Domain.Entities;

public class FacebookOAuthSession
{
    public string Key { get; set; } = string.Empty; // PK — random hex string
    public FacebookOAuthPhase Phase { get; set; }
    public Guid ProjectId { get; set; }
    public Guid TenantId { get; set; }
    public string? LongLivedToken { get; set; }       // null during State phase
    public DateTime? TokenExpiresAt { get; set; }
    public string? AdAccountsJson { get; set; }       // serialized List<FacebookAdAccount>
    public DateTime ExpiresAt { get; set; }
}

public enum FacebookOAuthPhase { State, Session }
