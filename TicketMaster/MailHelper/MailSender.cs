using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Mail;

public class MailSender
{
    protected readonly ILogger Logger;

    public MailSender(ILogger<MailSender> logger)
    {
        Logger = logger;
    }

    public void SendEmail(MailArguments mailArgs)
    {
        mailArgs.SmtpHost = "smtp.gmail.com";
        mailArgs.Port = 587;
        mailArgs.MailTo = "mip@ciklum.com";

        MailMessage message = new MailMessage(mailArgs.MailFrom, mailArgs.MailTo);
        message.Subject = mailArgs.Subject;
        message.Body = mailArgs.Message;
        foreach (var attachment in mailArgs.Attachments)
        {
            message.Attachments.Add(attachment);
        }

        SmtpClient client = new SmtpClient(mailArgs.SmtpHost);
        // Credentials are necessary if the server requires the client
        // to authenticate before it will send email on the client's behalf.
        //client.UseDefaultCredentials = true;
        client.UseDefaultCredentials = false;
        var networkCredential = new NetworkCredential
        {
            Password = mailArgs.Password,
            UserName = mailArgs.MailFrom
        };
        client.Credentials = networkCredential;
        client.EnableSsl = true;
        client.Port = mailArgs.Port;
        client.DeliveryMethod = SmtpDeliveryMethod.Network;

        try
        {
            client.Send(message);
        }
        catch (Exception ex)
        {
            Logger.LogError("Exception caught in CreateMessage(): {0}", ex.ToString());
        }
    }
}