using DTO.User;

namespace DTO.Team
{
	public class GetTeamUpdate
	{
		public Guid Id { get; set; }
		public string name { get; set; }
		public List<UserPart> users { get; set; }
	}
}
