using API.Domain.DocumentManagement.DocumentAggregate;
using API.Domain.ValueObjects;
using API.Dtos.Read;
using API.Dtos.Write;

namespace API.Mappers
{
	public static class DocumentMapper
	{
		public static Document DtoToDocument(DocumentW documentW)
		{
			DocumentName documentName = new DocumentName(documentW.DocumentName);
			Document document = new Document(documentName, documentW.Text);
			return document;
		}

		public static DocumentR DocumentToDto(Document document)
		{
			DocumentR documentR = new DocumentR
			{
				Id = document.Id,
				DocumentName = document.DocumentName.Value,
				Text = document.Text,
				OwnerId = document.OwnerId,
				Editors = document.Editors.Select(x => x.Username.Value).ToList()
			};

			return documentR;
		}
	}
}
