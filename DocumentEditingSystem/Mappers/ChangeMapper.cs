using API.Domain.Core.DocumentAggregate;
using API.Dtos.Read;
using API.Dtos.Write;

namespace API.Mappers
{
	public static class ChangeMapper
	{
		public static ChangeR ChangeToDto(Change change)
		{
			ChangeR changeR = new ChangeR
			{
				DocumentId = change.DocumentId,
				EditorId = change.EditorId,
				ChangePosition = change.ChangePosition,
				ChangeType = change.ChangeType,
				Text = change.Text,
				Date = change.Date
			};

			return changeR;
		}

		public static Change DtoToChange(ChangeW changeW)
		{
			Change change = new Change(changeW.EditorId, changeW.ChangePosition, changeW.Text, changeW.ChangeType);
			return change;
		}
	}
}
