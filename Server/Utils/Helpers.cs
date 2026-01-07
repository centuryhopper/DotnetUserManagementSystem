using Microsoft.AspNetCore.Mvc.ModelBinding;
using MimeKit;
using MailKit.Net.Smtp;
using MimeKit.Text;


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
}
