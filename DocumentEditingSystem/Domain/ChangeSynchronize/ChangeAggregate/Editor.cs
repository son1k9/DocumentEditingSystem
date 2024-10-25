using API.Domain.ValueObjects;

namespace API.Domain.ChangeSynchronize.ChangeAggregate
{
	internal class Editor
	{
		public int Id { get; }
		public int EditorId { get; private set; }
		
		public Editor(int editorId)
		{
			EditorId = editorId;
		}

	}
}
