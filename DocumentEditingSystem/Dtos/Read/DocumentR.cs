namespace API.Dtos.Read
{
	public class DocumentR
	{
		public int Id { get; set; }
		public string DocumentName { get; set; }

		public string Text { get; set; }
		public int OwnerId{ get; set; }

	}
}
