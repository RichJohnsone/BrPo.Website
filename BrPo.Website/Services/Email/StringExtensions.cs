using System.ComponentModel.DataAnnotations;

namespace BrPo.Website.Services.Email
{
    public static class StringExtensions
    {
        public static bool IsValidEmailAddress(this string address) => address != null && new EmailAddressAttribute().IsValid(address);
    }
}
