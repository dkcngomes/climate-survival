using MailKit.Net.Smtp;
using MimeKit;

namespace ClimateAdvisor.Api.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendContactNotificationAsync(string name, string email, string subject, string message, CancellationToken ct = default)
    {
        var smtpHost = _config["Email:SmtpHost"];
        var smtpPortStr = _config["Email:SmtpPort"];
        var smtpUser = _config["Email:SmtpUser"];
        var smtpPass = _config["Email:SmtpPass"];
        var toAddress = _config["Email:ToAddress"] ?? "dkcngomes@gmail.com";

        if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
        {
            _logger.LogWarning("SMTP not configured — contact message from {Name} <{Email}> not sent (subject: {Subject})", name, email, subject);
            return;
        }

        var smtpPort = int.TryParse(smtpPortStr, out var p) ? p : 587;

        try
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("Climate Survival Contact", smtpUser));
            mimeMessage.To.Add(new MailboxAddress("Nipuna Gomes", toAddress));
            mimeMessage.Subject = $"[Climate Survival] {subject}";

            var body = new TextPart("plain")
            {
                Text = $"""
New contact form submission:

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
From:    {name} ({email})
Subject: {subject}
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

{message}

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Sent via Climate Survival Contact Form
"""
            };

            mimeMessage.Body = body;

            using var client = new SmtpClient();

            // Accept all SSL certificates for Gmail SMTP
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            await client.ConnectAsync(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.StartTls, ct);
            await client.AuthenticateAsync(smtpUser, smtpPass, ct);
            await client.SendAsync(mimeMessage, ct);
            await client.DisconnectAsync(true, ct);

            _logger.LogInformation("Contact email sent successfully from {Email}, subject: {Subject}", email, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send contact email from {Email}, subject: {Subject}", email, subject);
            // Don't throw — we don't want to fail the API response if email fails
        }
    }
}
