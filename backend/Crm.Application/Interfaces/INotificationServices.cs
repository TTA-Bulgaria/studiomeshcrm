namespace Crm.Application.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendTemplatedEmailAsync<T>(string to, string subject, string templateName, T model) where T : notnull;
}

public interface ISlackService
{
    Task SendNotificationAsync(string message);
}
