using BrPo.Website.Data;
using BrPo.Website.Services.ContactForm.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using BrPo.Website.Services.Email;
using Microsoft.Extensions.Logging;

namespace BrPo.Website.Services.ContactForm.Services;

public interface IContactService
{
    Task Save(Contact contact);

    Task<Contact> Find(Contact contact);

    Task Email(Contact contact);
}

public class ContactService : IContactService
{
    private readonly ApplicationDbContext context;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<ContactService> _logger;

    public ContactService(
        ApplicationDbContext applicationDbContext,
        IEmailSender emailSender,
        ILogger<ContactService> logger)
    {
        context = applicationDbContext;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task Save(Contact contact)
    {
        await Task.Delay(20);
        context.Contacts.Add(contact);
        await context.SaveChangesAsync();
    }

    public Task<Contact> Find(Contact contact)
    {
        var contacts = context.Contacts.Where(
            c => c.Email == contact.Email
                 && c.Name == contact.Name
                 && c.Message == contact.Message);
        if (contacts.Any())
        {
            return Task.FromResult(contacts.OrderBy(c => c.Id).Last());
        }
        return null;
    }

    public async Task Email(Contact contact)
    {
        var plainTextBody = new StringBuilder();
        plainTextBody.AppendLine($"Name: {contact.Name}");
        plainTextBody.AppendLine(" ");
        plainTextBody.AppendLine($"Email: {contact.Email}");
        plainTextBody.AppendLine(" ");
        plainTextBody.AppendLine($"Message: {contact.Message}");

        var htmlBody = new StringBuilder();
        htmlBody.AppendLine("<html><head></head><body>");
        htmlBody.AppendLine($"<p>Name: {contact.Name}</p>");
        htmlBody.AppendLine($"<p>Email: {contact.Email}</p>");
        htmlBody.AppendLine($"<p>Message: {contact.Message}</p>");
        htmlBody.AppendLine("</body></html>");

        await _emailSender.SendEmailAsync(
            "Info@BrixtonPhotographic.com",
            "New Contact Enquiry",
            plainTextBody.ToString(),
            "Info@BrixtonPhotographic.com",
            "BrPo New Contact Enquiries",
            htmlBody.ToString());
    }
}