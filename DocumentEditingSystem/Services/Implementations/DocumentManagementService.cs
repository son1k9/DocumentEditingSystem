using API.Domain.DocumentManagement.DocumentAggregate;
using API.Domain.ValueObjects;
using API.Services.Interfaces;
using System.Text;

namespace API.Services.Implementations
{
	public class DocumentManagementService: IDocumentManagementService
	{
		public void DeleteDocument()
		{
			throw new NotImplementedException();
		}

		public async void LoadDocument(IFormFile documentFile, DocumentName documentName)
		{
			string text;

			using (var reader = new StreamReader(documentFile.OpenReadStream()))
			{
				text = await reader.ReadToEndAsync();
			}

			
			Document document = new Document(documentName, text);
			
		}

		public void SetManager()
		{
			throw new NotImplementedException();
		}

		public void UpdateDocument(IFormFile newDocumentText, int documentId)
		{
			throw new NotImplementedException();
		}
	}
}
