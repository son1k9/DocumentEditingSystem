using API.Domain.ValueObjects;

namespace API.Domain.Core.DocumentAggregate
{
	internal class EditingDocument
	{
		public int id { get; }
		public DocumentName DocumentName { get; }
		public string Text { get; private set; }
		public List<DocumentEditor> Editors { get; private set; }
		
		public void AddEditor(DocumentEditor editor)
		{
			if (editor == null) throw new ArgumentNullException("Editor cannot be null");
			if (Editors.Contains(editor)) throw new ArgumentException("Editor has already been added");
			Editors.Add(editor);
		}

		public void RemoveEditor(DocumentEditor editor)
		{
			if (editor == null) throw new ArgumentNullException("Editor cannot be null");
			if (!Editors.Contains(editor)) throw new ArgumentException("Editor does not have access to the document");
			Editors.Remove(editor);
		}

		public string ReadText(DocumentEditor documentEditor)
		{
			if (!Editors.Contains(documentEditor)) throw new ArgumentException("Editor does not have access to the document");
			return Text;
		}

		public void EditText(DocumentEditor documentEditor, string text)
		{
			
		}

	}
}
