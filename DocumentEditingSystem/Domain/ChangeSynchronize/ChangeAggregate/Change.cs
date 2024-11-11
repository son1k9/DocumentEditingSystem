namespace API.Domain.ChangeSynchronize.ChangeAggregate
{
	internal class Change
	{
		public int Id { get; }
		public string ChangingDocument { get; }
		public string EditorEmail { get; }

	}
}
