using System;
using System.Net.Mail;

public class MailHelper
{
    public static void SendEmail()
    {
        var mailArgs = new MailArguments
        {
            MailFrom = "",
            Password = "<--From mail address password-->",
            Name = "<--From mail address name-->",
            MailTo = "",
            Subject = "<--Subject of the email-->",
            Message = "<--Message body of the email can contains HTML as well-->",
            Port = 587,
            SmtpHost = "smtp.gmail.com",
            Bcc = "<--BCC email id's separated by semicolon (;)-->"
        };

        MailMessage message = new MailMessage(mailArgs.MailFrom, mailArgs.MailTo);
        message.Subject = mailArgs.Subject;
        message.Body = mailArgs.Message;
        SmtpClient client = new SmtpClient(mailArgs.SmtpHost);
        // Credentials are necessary if the server requires the client
        // to authenticate before it will send email on the client's behalf.
        client.UseDefaultCredentials = true;

        try
        {
            client.Send(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception caught in CreateMessage(): {0}",
                ex.ToString());
        }
    }
}