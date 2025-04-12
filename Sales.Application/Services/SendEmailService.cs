using FluentEmail.Core;
using FluentEmail.Core.Models;
using Microsoft.Extensions.Logging;
using Sales.Application.Interfaces;

namespace Sales.Application.Services;

public class SendEmailService : ISendEmailService
{
    private readonly IFluentEmail _fluentEmail;
    private readonly ILogger<SendEmailService> _logger;

    public SendEmailService(IFluentEmail fluentEmail, ILogger<SendEmailService> logger)
    {
        _fluentEmail = fluentEmail;
        _logger = logger;
    }

    public async Task SendEmailAsync(
        string email, 
        string subject, 
        string body, 
        Attachment attachment)
    {
        await _fluentEmail
            .To(email)
            .Subject(subject)
            .Body(body)
            .Attach(attachment)
            .SendAsync();
        
        _logger.LogInformation("\n****** Email sent to {Email} ******\n", email);
    }
}