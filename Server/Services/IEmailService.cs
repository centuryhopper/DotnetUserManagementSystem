

using MimeKit.Text;
using Radzen.Blazor.Markdown;

namespace Server.Services;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body, TextFormat textFormat = TextFormat.Html);
}