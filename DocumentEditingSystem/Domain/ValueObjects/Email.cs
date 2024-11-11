using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API.Domain.ValueObjects
{
    public class Email
    {
        public string Value;

        public Email(string value)
        {
            try
            {
                var email = new MailAddress(value);
                Value = value;
            }
            catch
            {
                throw new ArgumentException("Email is invalid");
            }

        }
    }
}
