using System;
using System.ComponentModel.DataAnnotations;

namespace BrPo.Website.Services.Email
{
    public static class StringExtensions
    {
        public static bool IsValidEmailAddress(this string address) => address != null && new EmailAddressAttribute().IsValid(address);

        public static int ToInt(this string s) => Convert.ToInt32(s);

        public static decimal ToCurrency(this string s) => Math.Round(Convert.ToDecimal(s), 2);
    }
}
