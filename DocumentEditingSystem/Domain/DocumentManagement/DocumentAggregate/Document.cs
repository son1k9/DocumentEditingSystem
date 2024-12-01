using API.Domain.ValueObjects;
using API.Domain.ValueObjects.Enums;
using Domain.UserManagement.UserAggregate;
using MediatR;

namespace API.Domain.DocumentManagement.DocumentAggregate
{
    public class Document
	{
		public int Id { get;}
		public DocumentName DocumentName { get; private set; }
		public DocumentContent Content { get; private set; }
		public DateTime CreationDate { get; private set; }
		public DateTime ChangingDate { get; private set; }
		public int OwnerId { get; private set; }
		public ICollection<User> Editors { get; private set; } = [];
		public int Version { get; set; }

		public Document(DocumentName documentName, string text)
		{
			if (documentName == null) throw new ArgumentNullException("Document name cannot be null");
			DocumentName = documentName;
			Content = new DocumentContent
			{
				Text = text
			};
			CreationDate = DateTime.UtcNow;
			ChangingDate = DateTime.UtcNow;
			Version = 0;
		}

		public void SetOwnerId(int ownerId)
		{
			OwnerId = ownerId;
		}

		public void SetEditors(ICollection<User> editors)
		{
			Editors = editors;
		}

		public void UpdateDocument(string text, int newVersion)
		{
			Version = newVersion;
			Content.Text = text;
			ChangingDate = DateTime.UtcNow;
		}

		private Document() { }
	}
}
