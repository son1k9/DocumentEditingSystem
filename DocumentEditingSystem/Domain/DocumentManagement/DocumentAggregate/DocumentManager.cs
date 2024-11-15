using API.Domain.ValueObjects;
using API.Domain.ValueObjects.Enums;

namespace API.Domain.DocumentManagement.DocumentAggregate
{
    public class DocumentManager
    {
        public int Id { get; }

        public Username Username { get; }

        public List<Document> Documents { get; }

        public DocumentManager(int userId, Username username) {
			if (username == null) throw new ArgumentException("Username can not be null");
			Id = userId;
            Username = username;
        }

        public void AddDocument(Document document)
        {
            if (document == null) throw new ArgumentNullException("Document can not be null!");
            Documents.Add(document);
        }

        public void UpdateDocument(Document document, string text)
        {
            int index = Documents.IndexOf(document);
            document.UpdateDocument(text);
            Documents[index] = document;
        }

        public void RemoveDocument(Document document)
        {
            if (document == null) throw new ArgumentNullException("Document can not be null!");
            if (!Documents.Contains(document)) throw new ArgumentException("Document not found!");
            Documents.Remove(document);
        }

        private DocumentManager() { }

    }
}
