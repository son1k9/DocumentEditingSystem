using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API.Domain.ValueObjects
{
    internal class Name
    {
        public string Value { get; }

        private static readonly Regex ValidationRegex = new Regex(
            @"^[\p{L}\p{M}\p{N}]{1,120}\z",
            RegexOptions.Singleline | RegexOptions.Compiled);

        public Name(string value)
        {
            if (!IsValid(value))
            {
                throw new ArgumentException("Name is not valid");
            }

            Value = value;
        }

        public static bool IsValid(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && ValidationRegex.IsMatch(value);
        }
    }
}
