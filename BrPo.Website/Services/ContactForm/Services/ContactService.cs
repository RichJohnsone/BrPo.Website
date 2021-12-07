using BrPo.Website.Services.ContactForm.Models;
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
        public async Task Save(ContactModel contactModel)
        {
            await Task.Delay(20);
        }

        public Task<ContactModel> Find(ContactModel contactModel)
        {
            return Task.FromResult(contactModel);
        }
    }
}
