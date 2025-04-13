using FluentEmail.Core.Models;

namespace Sales.Application.Interfaces;

public interface ISendEmailService
{
    Task SendEmailAsync(string destination, string subject, string body, string tag, Attachment attachment);
}