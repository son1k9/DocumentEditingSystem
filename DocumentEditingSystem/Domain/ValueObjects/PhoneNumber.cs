using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API.Domain.ValueObjects
{
    public class PhoneNumber
    {

        public string Value { get; }
        public static readonly Regex regex = new Regex(
            @"^\(?([7-8]{1})\)?[-. ]?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
            RegexOptions.Singleline | RegexOptions.Compiled);

        public PhoneNumber(string value)
        {
            if (!IsValid(value))
            {
                throw new ArgumentException("Phone number is not valid");
            }
            Value = value;

        }

        public static bool IsValid(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && regex.IsMatch(value);
        }
    }
}
