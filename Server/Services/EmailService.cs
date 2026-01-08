

using MimeKit.Text;
using Resend;

namespace Server.Services;

public class EmailService : IEmailService
{
    private readonly IResend resend;
    private readonly IWebHostEnvironment env;
    private readonly string fromEmail;

    public EmailService(IResend resend, IConfiguration configuration, IWebHostEnvironment env)
    {
        this.resend = resend;
        this.env = env;
        this.fromEmail = env.IsDevelopment() ? configuration["Resend:FromMail"] : Environment.GetEnvironmentVariable("Resend_FromMail")!;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body, TextFormat textFormat = TextFormat.Html)
    {
        var message = new EmailMessage();
        message.From = fromEmail;
        message.To.Add(toEmail);
        message.Subject = subject;

        if (textFormat == TextFormat.Plain)
            message.TextBody = body;
        else if (textFormat == TextFormat.Html)
            message.HtmlBody = body;

        await resend.EmailSendAsync(message);
    }
}