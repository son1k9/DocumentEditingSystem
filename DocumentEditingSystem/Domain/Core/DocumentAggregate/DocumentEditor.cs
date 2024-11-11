using API.Domain.ValueObjects;

namespace API.Domain.Core.DocumentAggregate
{
	public class DocumentEditor
	{
		public int Id { get; }
		public Email Email { get; private set; }

	}
}
