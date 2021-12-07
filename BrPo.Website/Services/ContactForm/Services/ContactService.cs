using BrPo.Website.Data;
using BrPo.Website.Services.ContactForm.Models;
using System.Linq;
using System.Threading.Tasks;

namespace BrPo.Website.Services.ContactForm.Services
{
    public interface IContactService
    {
        Task Save(ContactModel contactModel);
        Task<ContactModel> Find(ContactModel contactModel);
    }

    public class ContactService : IContactService
    {
        private readonly ApplicationDbContext context;

        public ContactService(ApplicationDbContext applicationDbContext)
        {
            context = applicationDbContext;
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
            if (contacts.Any()) {
                return Task.FromResult(contacts.OrderBy(c => c.Id).Last());
            }
            return null;
        }
    }
}
