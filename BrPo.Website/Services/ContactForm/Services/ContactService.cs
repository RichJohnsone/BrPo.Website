using BrPo.Website.Data;
using BrPo.Website.Services.ContactForm.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using BrPo.Website.Services.Email;
using Microsoft.Extensions.Logging;

namespace BrPo.Website.Services.ContactForm.Services
{
    public interface IContactService
    {
        Task Save(ContactModel contactModel);

        Task<ContactModel> Find(ContactModel contactModel);

        Task Email(ContactModel contactModel);
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

        public async Task Save(ContactModel contactModel)
        {
            await Task.Delay(20);
            context.Contacts.Add(contactModel);
            await context.SaveChangesAsync();
        }

        public Task<ContactModel> Find(ContactModel contactModel)
        {
            var contacts = context.Contacts.Where(
                c => c.Email == contactModel.Email
                && c.Name == contactModel.Name
                && c.Message == contactModel.Message);
            if (contacts.Any())
            {
                return Task.FromResult(contacts.OrderBy(c => c.Id).Last());
            }
            return null;
        }

        public async Task Email(ContactModel contactModel)
        {
            var plainTextBody = new StringBuilder();
            plainTextBody.AppendLine($"Name: {contactModel.Name}");
            plainTextBody.AppendLine(" ");
            plainTextBody.AppendLine($"Email: {contactModel.Email}");
            plainTextBody.AppendLine(" ");
            plainTextBody.AppendLine($"Message: {contactModel.Message}");

            var htmlBody = new StringBuilder();
            htmlBody.AppendLine("<html><head></head><body>");
            htmlBody.AppendLine($"<p>Name: {contactModel.Name}</p>");
            htmlBody.AppendLine($"<p>Email: {contactModel.Email}</p>");
            htmlBody.AppendLine($"<p>Message: {contactModel.Message}</p>");
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
}