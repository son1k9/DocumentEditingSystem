using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API.Domain.ValueObjects
{
    public class Name
    {
        public string FirstName { get; }
		public string LastName { get; }

		private static readonly Regex ValidationRegex = new Regex(
            @"^[\p{L}\p{M}]{1,120}\z",
            RegexOptions.Singleline | RegexOptions.Compiled);

        public Name(string firstName, string lastName)
        {
            if (!IsValid(firstName))
            {
                throw new ArgumentException("First name is not valid");
            }

			if (!IsValid(lastName))
			{
				throw new ArgumentException("Last name is not valid");
			}

			FirstName = firstName;
			LastName = lastName;
		}

        public static bool IsValid(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && ValidationRegex.IsMatch(value);
        }
    }
}
