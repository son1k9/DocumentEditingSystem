using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.TokenConfig
{
	public static class AuthOptions
	{
		public const string ISSUER = "DocumentEditingSystem";
		public const string AUDIENCE = "DocumentEditingSystem";
		const string KEY = "DocumentEditingSystemSecurityKey";
		public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
			new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
	}
}
