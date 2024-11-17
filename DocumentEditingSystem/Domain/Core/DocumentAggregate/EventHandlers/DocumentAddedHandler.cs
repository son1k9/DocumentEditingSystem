using API.Domain.DocumentManagement.Events;
using API.Infrastructure.Repositories.Interfaces;
using MediatR;

namespace API.Domain.Core.DocumentAggregate.EventHandlers
{
	public class DocumentAddedHandler : INotificationHandler<DocumentAdded>
	{
		private readonly IDocumentEditingRepository _documentEditingRepository;

		public DocumentAddedHandler(IDocumentEditingRepository documentEditingRepository)
		{
			_documentEditingRepository = documentEditingRepository;
		}

		public async Task Handle(DocumentAdded notification, CancellationToken cancellationToken)
		{
			EditingDocument editingDocument = new EditingDocument(notification.DocumentId, notification.OwnerId);
			await _documentEditingRepository.AddDocumentAsync(editingDocument);
		}
	}
}
