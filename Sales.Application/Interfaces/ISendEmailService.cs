using FluentEmail.Core.Models;

namespace Sales.Application.Interfaces;

public interface ISendEmailService
{
    Task SendEmailAsync(string email, string subject, string body, Attachment attachment);
}