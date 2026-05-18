using Crm.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Crm.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IEmailTemplateRenderer _renderer;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration, IEmailTemplateRenderer renderer)
    {
        _logger = logger;
        _configuration = configuration;
        _renderer = renderer;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        await SendRawAsync(to, subject, body);
    }

    public async Task SendTemplatedEmailAsync<T>(string to, string subject, string templateName, T model) where T : notnull
    {
        var body = await _renderer.RenderAsync(templateName, model);
        await SendRawAsync(to, subject, body);
    }

    private async Task SendRawAsync(string to, string subject, string htmlBody)
    {
        var smtp = _configuration.GetSection("SmtpSettings");
        var host = smtp["Host"] ?? throw new InvalidOperationException("SmtpSettings:Host is not configured.");
        var port = int.Parse(smtp["Port"] ?? "465");
        var username = smtp["Username"] ?? throw new InvalidOperationException("SmtpSettings:Username is not configured.");
        var password = smtp["Password"] ?? throw new InvalidOperationException("SmtpSettings:Password is not configured.");
        var fromName = smtp["FromName"] ?? "Studio Mesh CRM";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, username));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(host, port, SecureSocketOptions.SslOnConnect);
            await client.AuthenticateAsync(username, password);
            await client.SendAsync(message);
        }
        finally
        {
            await client.DisconnectAsync(true);
        }

        _logger.LogInformation("Email sent to {To} — Subject: {Subject}", to, subject);
    }
}
