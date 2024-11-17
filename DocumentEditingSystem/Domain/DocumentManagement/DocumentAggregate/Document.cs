using API.Domain.ValueObjects;
using API.Domain.ValueObjects.Enums;
using MediatR;

namespace API.Domain.DocumentManagement.DocumentAggregate
{
    public class Document
	{
		public int Id { get;}
		public DocumentName DocumentName { get; private set; }
		public string Text { get; private set; }
		public DateTime CreationDate { get; private set; }
		public DateTime ChangingDate { get; private set; }
		public int OwnerId { get; private set; }

		public Document(DocumentName documentName, string text)
		{
			if (documentName == null) throw new ArgumentNullException("Document name cannot be null");
			DocumentName = documentName;
			Text = text;
			CreationDate = DateTime.UtcNow;
			ChangingDate = DateTime.UtcNow;
		}

		public void SetOwnerId(int ownerId)
		{
			OwnerId = ownerId;
		}

		public void UpdateDocument(string text)
		{
			Text = text;
			ChangingDate = DateTime.UtcNow;
		}

		private Document() { }

		
	}
}
