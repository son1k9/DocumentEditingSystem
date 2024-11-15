using API.Domain.ValueObjects;

namespace API.Dtos.Write
{
	public class DocumentW
	{
		public int Id { get; set; }
		public string DocumentName { get; private set; }
		public string Text { get; private set; }
	}
}
