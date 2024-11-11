using System.Text.RegularExpressions;

namespace API.Domain.ValueObjects
{
	public class Username
	{
		public string Value {  get; private set; }

		private static readonly Regex ValidationRegex = new Regex(
			@"([0-9a-zA-Z_]){6,12}\z",
			RegexOptions.Singleline | RegexOptions.Compiled);

		public Username(string username)
		{
			if (!IsValid(username))
			{
				throw new ArgumentException("Username is invalid!");
			}
			Value = username;
		}

		public static bool IsValid(string value)
		{
			return !string.IsNullOrWhiteSpace(value) && ValidationRegex.IsMatch(value);
		}

		private Username() { }
	}
}
