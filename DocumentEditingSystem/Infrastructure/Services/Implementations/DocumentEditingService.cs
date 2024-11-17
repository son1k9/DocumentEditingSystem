using API.Domain.Core.DocumentAggregate;
using API.Dtos.Read;
using API.Dtos.Write;
using API.Infrastructure.Repositories.Interfaces;
using API.Infrastructure.Services.Interfaces;
using API.Mappers;
using Domain.UserManagement.UserAggregate;

namespace API.Infrastructure.Services.Implementations
{
	public class DocumentEditingService : IDocumentEditingService
	{
		private readonly IDocumentEditingRepository _documentEditingRepository;
		public DocumentEditingService(IDocumentEditingRepository documentEditingRepository)
		{
			_documentEditingRepository = documentEditingRepository;
		}

		public async Task<List<ChangeR>> AddChangesToDocument(List<ChangeW> changes, int documentId, int userId)
		{
			EditingDocument editingDocument = await _documentEditingRepository.GetDocumentByIdAsync(documentId);

			if(editingDocument == null)
			{
				throw new ArgumentException("Document not found!");
			}
			
			foreach(ChangeW changeW in changes)
			{
				editingDocument.AddChange(ChangeMapper.DtoToChange(changeW));
			}

			var result = await _documentEditingRepository.UpdateDocumentAsync(editingDocument);

			if (!result)
			{
				throw new Exception("Failed to add changes!");
			}

			List<ChangeR> changesR = new List<ChangeR>();

			foreach(Change change in editingDocument.Changes)
			{
				changesR.Add(ChangeMapper.ChangeToDto(change));
			}

			return changesR;
		}

		public async Task AddEditor(int documentId, int editorId, int userId)
		{
			EditingDocument editingDocument = await _documentEditingRepository.GetDocumentByIdAsync(documentId);
			if (editingDocument == null) throw new ArgumentException("Document not found!");
			if (editingDocument.OwnerId != userId) throw new ArgumentException("The user does not have access to the document!");

			editingDocument.AddEditor(editorId);

			var result = await _documentEditingRepository.UpdateDocumentAsync(editingDocument);
			if (!result) throw new Exception("An error occurred updating the document");

		}

		public async Task<List<ChangeR>> GetDocumentChanges(int documentId, int userId)
		{
			EditingDocument editingDocument = await _documentEditingRepository.GetDocumentByIdAsync(documentId);

			if (editingDocument == null)
			{
				throw new ArgumentException("Document not found!");
			}

			if (editingDocument.OwnerId != userId)
			{
				throw new ArgumentException("The user does not have access to the document!");
			}

			List<ChangeR> result = new List<ChangeR>();
			foreach(Change change in editingDocument.Changes)
			{
				result.Add(ChangeMapper.ChangeToDto(change));
			}

			return result;
		}

		public async Task RemoveEditor(int documentId, int editorId, int userId)
		{
			EditingDocument editingDocument = await _documentEditingRepository.GetDocumentByIdAsync(documentId);
			if (editingDocument == null) throw new ArgumentException("Document not found!");
			if (editingDocument.OwnerId != userId) throw new ArgumentException("The user does not have access to the document!");

			editingDocument.RemoveEditor(editorId);

			var result = await _documentEditingRepository.UpdateDocumentAsync(editingDocument);
			if (!result) throw new Exception("An error occurred updating the document");
		}
	}
}
