using ClimateAdvisor.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace ClimateAdvisor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private static readonly ConcurrentBag<ContactMessage> Messages = new();
    private readonly IEmailService _email;
    private readonly ILogger<ContactController> _logger;

    public ContactController(IEmailService email, ILogger<ContactController> logger)
    {
        _email = email;
        _logger = logger;
    }

    /// <summary>POST /api/contact — submit a contact form message and forward to email</summary>
    [HttpPost]
    public IActionResult Submit([FromBody] ContactRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 200)
            return BadRequest(new { error = "Name is required (max 200 characters)" });

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@') || request.Email.Length > 320)
            return BadRequest(new { error = "A valid email is required" });

        if (string.IsNullOrWhiteSpace(request.Subject) || request.Subject.Length > 200)
            return BadRequest(new { error = "Subject is required (max 200 characters)" });

        if (string.IsNullOrWhiteSpace(request.Message) || request.Message.Length > 5000)
            return BadRequest(new { error = "Message is required (max 5000 characters)" });

        var msg = new ContactMessage
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            Subject = request.Subject.Trim(),
            Message = request.Message.Trim(),
            ReceivedAt = DateTime.UtcNow,
        };

        Messages.Add(msg);
        _logger.LogInformation("Contact message #{Id} from {Name} <{Email}>: {Subject}", msg.Id, msg.Name, msg.Email, msg.Subject);

        // Forward to email asynchronously (fire-and-forget — don't block the API response)
        Task.Run(async () =>
        {
            try
            {
                await _email.SendContactNotificationAsync(msg.Name, msg.Email, msg.Subject, msg.Message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Background email send failed for contact message #{Id}", msg.Id);
            }
        });

        return Ok(new { success = true, messageId = msg.Id });
    }
}

public class ContactRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class ContactMessage
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; }
}
