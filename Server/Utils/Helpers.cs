using Microsoft.AspNetCore.Mvc.ModelBinding;
using MimeKit;
using MailKit.Net.Smtp;
using MimeKit.Text;
using Microsoft.AspNetCore.Identity;
using System.Text;


namespace Server.Utils;

public static class Helpers
{
    public static void SendEmail(
        string subject,
        string body,
        string senderEmail,
        string senderPassword,
        IEnumerable<string> receivers,
        TextFormat textFormat = TextFormat.Plain
    )
    {
        var msg = new MimeMessage();

        msg.From.Add(new MailboxAddress("Automated Email Message", senderEmail));

        foreach (var receiver in receivers)
        {
            msg.To.Add(MailboxAddress.Parse(receiver));
        }

        msg.Subject = subject;

        msg.Body = new TextPart(textFormat) { Text = body, };

        var client = new SmtpClient();

        try
        {
            client.Connect("smtp.gmail.com", 465, true);
            client.Authenticate(senderEmail, senderPassword);
            var res = client.Send(msg);
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine(ex.Message);
        }
        finally
        {
            client.Disconnect(true);
            client.Dispose();
        }
    }

    public static async Task SendEmailAsync(
        string subject,
        string body,
        string senderEmail,
        string senderPassword,
        IEnumerable<string> receivers,
        TextFormat textFormat = TextFormat.Plain
    )
    {
        var msg = new MimeMessage();

        msg.From.Add(new MailboxAddress("Automated Email Message", senderEmail));

        foreach (var receiver in receivers)
        {
            msg.To.Add(MailboxAddress.Parse(receiver));
        }

        msg.Subject = subject;

        msg.Body = new TextPart(textFormat) { Text = body, };

        var client = new SmtpClient();
        client.Timeout = 10_000; // 10 seconds

        try
        {
            await client.ConnectAsync("smtp.gmail.com", 465, true);
            await client.AuthenticateAsync(senderEmail, senderPassword);
            var res = await client.SendAsync(msg);
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine(ex.Message);
        }
        finally
        {
            await client.DisconnectAsync(true);
            client.Dispose();
        }
    }

    public static string Build2FAHtmlEmail(IdentityUser identityUser, string twoFaToken)
    {
        StringBuilder emailBodyBuilder = new();
        emailBodyBuilder.AppendLine("<html>");
        emailBodyBuilder.AppendLine("<head>");
        emailBodyBuilder.AppendLine("<style>");
        emailBodyBuilder.AppendLine("body { font-family: Arial, sans-serif; color: #333; margin: 20px; }");
        emailBodyBuilder.AppendLine("h1 { color: #007bff; }");
        emailBodyBuilder.AppendLine("p { margin: 10px 0; }");
        emailBodyBuilder.AppendLine(".code { font-size: 24px; font-weight: bold; color: #28a745; }");
        emailBodyBuilder.AppendLine("</style>");
        emailBodyBuilder.AppendLine("</head>");
        emailBodyBuilder.AppendLine("<body>");

        // Greeting
        emailBodyBuilder.AppendLine($"<p>Dear {identityUser.Email},</p>");

        // Main content
        emailBodyBuilder.AppendLine("<p>Thank you for using our application!</p>");
        emailBodyBuilder.AppendLine("<p>To complete your login process, please use the following verification code:</p>");

        // Verification code
        emailBodyBuilder.AppendLine($"<p class='code'>{twoFaToken}</p>");

        // Instructions
        emailBodyBuilder.AppendLine("<p>This code is valid for a short period, so please use it promptly.</p>");
        emailBodyBuilder.AppendLine("<p>If you did not request this code, please ignore this email.</p>");

        emailBodyBuilder.AppendLine("</body>");
        emailBodyBuilder.AppendLine("</html>");



        return emailBodyBuilder.ToString();

    }
}
