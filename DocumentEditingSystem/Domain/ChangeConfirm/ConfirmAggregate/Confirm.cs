using API.Domain.ValueObjects;

namespace API.Domain.ChangeConfirm.ConfirmAggregate
{
	internal class Confirm
	{
		public Name UserName { get; set; }
		public DocumentName DocumentName { get; set; }
		public DateTime ChangingDate { get; set; }

		public string Text { get; private set; }

		public Confirm(Name userName, DocumentName documentName, DateTime changingDate)
		{
			if (userName == null) throw new ArgumentNullException("Name cannot be null!");
			if (documentName == null) throw new ArgumentNullException("Document name cannot be null!");
			UserName = userName;
			DocumentName = documentName;
			ChangingDate = changingDate;

			Text = $"Document {DocumentName.Value} was changed by {userName.Value} at {ChangingDate}";
		}
	}
}
