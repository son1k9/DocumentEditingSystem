using API.Domain.ValueObjects.Enums;

namespace API.Dtos.Read
{
	public class ChangeR
	{
		public int DocumentId { get; set; }
		public int EditorId { get; set; }
		public int ChangePosition { get; set; }
		public string Text { get; set; }
		public ChangeType ChangeType { get; set; }
		public DateTime Date { get; set; }
	}
}
