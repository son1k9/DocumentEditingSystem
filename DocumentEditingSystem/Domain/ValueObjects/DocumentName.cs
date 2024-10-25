using System.Text.RegularExpressions;

namespace API.Domain.ValueObjects
{
    public class DocumentName
    {
        public string Value { get; }

        public DocumentName(string value)
        {
            if (!IsValid(value))
            {
                throw new ArgumentException("FileName is invalid");
            }
            Value = value;
        }

        public bool IsValid(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && value.Length > 3;
        }
    }
}
