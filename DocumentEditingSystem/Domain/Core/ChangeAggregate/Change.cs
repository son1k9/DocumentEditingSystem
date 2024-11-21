using API.Domain.ValueObjects.Enums;
using MediatR;

namespace API.Domain.Core.DocumentAggregate
{
	public class Change
	{
		public int Id { get; }
		public int DocumentId { get; private set; }
		public int EditorId { get; private set; }
		public int ChangePosition { get; private set; }
		public string Text { get; private set; }
		public ChangeType ChangeType { get; private set; }
        public int Version { get; private set; }
        public DateTime Date {  get; private set; }

		public Change(int editorId, int changePosition, string text, ChangeType changeType, int version)
		{
			EditorId = editorId;
			ChangePosition = changePosition;
			Text = text;
			ChangeType = changeType;
			Version = version;
			Date = DateTime.UtcNow;
		}
	}
}
