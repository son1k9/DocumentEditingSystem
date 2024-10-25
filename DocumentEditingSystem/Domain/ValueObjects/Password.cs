using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.Domain.ValueObjects
{
    internal class Password
    {
        public byte[] Hash { get; private set; }

        public Password(string password)
        {
            if (!IsValid(password))
            {
                throw new ArgumentException("Password is invalid");
            }

            HMACSHA512 hmac = new HMACSHA512(Encoding.UTF8.GetBytes("DocumentEditingSystem"));
            Hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public bool IsValid(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && password.Length > 8;
        }

        public bool Compare(string password)
        {
            if (!IsValid(password))
            {
                throw new ArgumentException("Password is invalid");
            }

            HMACSHA512 hmac = new HMACSHA512(Encoding.UTF8.GetBytes("DocumentEditingSystem"));
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            return hash.SequenceEqual(Hash);
        }
    }
}
