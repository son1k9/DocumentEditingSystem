using API.Domain.ValueObjects;

namespace API.Domain.Core.DocumentAggregate
{
	public class EditingDocument
	{
		public int Id { get; }
		public int OwnerId { get; }
		//public List<int> Editors { get; private set; }
		public List<Change> Changes { get; private set; }

		public EditingDocument(int documentId, int ownerId)
		{
			Id = documentId;
			OwnerId = ownerId;
			//Editors = new List<int>();
			Changes = new List<Change>();
		}

		public void AddEditor(int editorId)
		{
			//if (Editors.Contains(editorId)) throw new ArgumentException("The user has already been added!");
			//Editors.Add(editorId);
		}

		public void RemoveEditor(int editorId)
		{
			//if (!Editors.Contains(editorId)) throw new ArgumentException("User not found!");
			//Editors.Remove(editorId);
		}

		public void AddChange(Change change)
		{
			//if (!Editors.Contains(change.EditorId) & change.EditorId != OwnerId) throw new ArgumentException("The user does not have access to the document!");
			if (change == null) throw new ArgumentNullException("Change can not be null!");
			Changes.Add(change);
		}

		private EditingDocument()
		{
			//Editors = new List<int>();
			Changes = new List<Change>();
		}
	}
}
