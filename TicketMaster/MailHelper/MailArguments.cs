using System.Collections.Generic;
using System.Net.Mail;

public class MailArguments
{
    public string MailFrom { get; set; }
    public string MailTo { get; set; }
    public string Bcc { get; set; }
    public string Password { get; set; }
    public string Subject { get; set; }
    public string Message { get; set; }
    public List<Attachment> Attachments { get; set; } = new List<Attachment>() { };
    public string SmtpHost { get; set; }
    public int Port { get; set; }
}