using API.Domain.ValueObjects;
using API.Domain.ValueObjects.Enums;

namespace API.Domain.DocumentManagement.DocumentAggregate
{
    public class Document
	{
		public int Id { get;}
		public DocumentName DocumentName { get; private set; }
		public string Text { get; private set; }
		public DateTime CreationDate { get; private set; }
		public DateTime ChangingDate { get; private set; }
		public DocumentManager Manager { get; private set; }

		public Document(DocumentName documentName, string text)
		{
			if (documentName == null) throw new ArgumentNullException("Document name cannot be null");
			DocumentName = documentName;
			CreationDate = DateTime.UtcNow;
			ChangingDate = DateTime.UtcNow;
		}

		public void SetManager(DocumentManager manager)
		{
			if(manager == null) throw new ArgumentNullException("Manager cannot be null");
			Manager = manager;
		}

		public void UpdateDocument(string text)
		{
			Text = text;
			ChangingDate = DateTime.UtcNow;
		}

		
	}
}
