using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.Domain.ValueObjects
{
    public class Password
    {
        public string Hash { get; private set; }

        public Password(string password)
        {
            if (!IsValid(password))
            {
                throw new ArgumentException("Password is invalid");
            }

            HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes("DocumentEditingSystem"));
            byte[] passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            Hash = Convert.ToHexString(passwordHash);
        }

        public bool IsValid(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && password.Length > 8;
        }

        public bool Compare(Password password)
        {
            if (password == null)
            {
                throw new ArgumentNullException("Password cannot be null");
            }

            return Hash == password.Hash;
        }

        private Password() { }
    }
}
