using MediatR;

namespace API.Domain.DocumentManagement.Events
{
	public class DocumentAdded : INotification
	{
		public int DocumentId { get; }
		public int OwnerId { get; }

		public DocumentAdded(int documentId, int ownerId)
		{
			DocumentId = documentId;
			OwnerId = ownerId;
		}
	}
}
