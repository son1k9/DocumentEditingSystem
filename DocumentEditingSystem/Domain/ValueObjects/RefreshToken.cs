using System.Security.Cryptography;
using System.Text;

namespace API.Domain.ValueObjects
{
	public class RefreshToken
	{
		public string Value { get; private set; }
		public bool TokenIsBlocked { get; private set; }

		private RefreshToken()
		{

		}

		public RefreshToken(string value)
		{
			HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes("DocumentEditingSystem"));
			byte[] tokenHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(value));
			Value = Convert.ToHexString(tokenHash);
			TokenIsBlocked = false;
		}

		public void BlockToken()
		{
			TokenIsBlocked = true;
		}

		public bool ValidateToken(RefreshToken refreshToken)
		{
			if (TokenIsBlocked) return false;

			return Value == refreshToken.Value;
		}
	}
}
