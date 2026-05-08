namespace Crm.Application.Email;

public record EmailVerificationModel(string FullName, string VerificationLink);
public record PasswordResetModel(string FullName, string ResetLink);
public record PasswordResetConfirmationModel(string FullName, string Email);
public record OfferSentModel(string ClientName, string AgencyName, string OfferLink);
public record ContractSentModel(string ClientName, string AgencyName, string ProjectName, string ContractLink);
public record ContractSignedModel(string ClientName, string ProjectName, string ContractValue, string SignedAt);
public record InvoiceGeneratedModel(string ClientName, string AgencyName, string InvoiceNumber, string AmountDue, string DueDate, string ProjectName, string PaymentLink);
public record InvoiceOverdueModel(string ClientName, string InvoiceNumber, string AmountDue, string DueDate, string PaymentLink);
