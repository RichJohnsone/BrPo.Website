using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace BrPo.Website.Services.Email;

public interface IEmailSender
{
    Task SendEmailAsync(string toEmail, string subject, string plainTextContent, string senderEmailAddress, string senderName = null, string htmlContent = null);
}

public class EmailSender : IEmailSender
{
    public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
    {
        Options = optionsAccessor.Value;
    }

    public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

    public Task SendEmailAsync(string toEmail, string subject, string plainTextContent, string senderEmailAddress, string senderName = null, string htmlContent = null)
    {
        if (string.IsNullOrEmpty(plainTextContent) && string.IsNullOrEmpty(htmlContent))
            throw new ApplicationException("BrPo.Website.Services.Email.SendEmailAsync: No content specified - cannot send empty email");
        var _senderEmailAddress = new EmailAddress(senderEmailAddress, senderName);
        return Execute(Options.SendGridKey, subject, plainTextContent, toEmail, _senderEmailAddress, htmlContent ?? plainTextContent);
    }

    private Task Execute(string apiKey, string subject, string plainTextContent, string toEmail, EmailAddress senderEmailAddress, string htmlContent)
    {
        var client = new SendGridClient(apiKey);
        var msg = new SendGridMessage()
        {
            From = senderEmailAddress,
            Subject = subject,
            PlainTextContent = plainTextContent,
            HtmlContent = htmlContent
        };
        msg.AddTo(new EmailAddress(toEmail));

        // Disable click tracking.
        // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
        msg.SetClickTracking(false, false);

        return client.SendEmailAsync(msg);
    }
}