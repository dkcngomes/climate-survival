namespace ClimateAdvisor.Api.Services;

public interface IEmailService
{
    Task SendContactNotificationAsync(string name, string email, string subject, string message, CancellationToken ct = default);
}
