using API.Domain.ValueObjects.Enums;

namespace API.Dtos.Write
{
	public class ChangeW
	{
		public int DocumentId { get; set; }
		public int EditorId { get; set; }
		public int ChangePosition { get; set; }
		public string Text { get; set; }
		public ChangeType ChangeType { get; set; }
	}
}
