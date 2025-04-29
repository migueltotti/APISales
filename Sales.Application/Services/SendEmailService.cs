using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.SendGrid;
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
        string destination, 
        string subject, 
        string body,
        string tag,
        Attachment attachment)
    {
        var response = await _fluentEmail
            .To(destination)
            .Subject(subject)
            .Body(body)
            .Tag(tag)
            .Attach(attachment)
            .SendAsync();

        if(response.Successful)
            _logger.LogInformation("Email sent successfully");
        else
            _logger.LogError("Error trying to sent email");
        
        /*var fluentEmail = _fluentEmail
            .To(destination)
            .Subject(subject)
            .Body(body)
            .Tag("report")
            .Attach(attachment);
        
        var sendGridSender = new SendGridSender("");
        var response = await sendGridSender.SendAsync(fluentEmail);
        
        if(response.Successful)
            _logger.LogInformation("\n****** Email sent to {Email} ******\n", destination);
        else 
            _logger.LogError("\n****** Error trying to sent email to destination: {Email} ******\n", destination);*/
    }
}