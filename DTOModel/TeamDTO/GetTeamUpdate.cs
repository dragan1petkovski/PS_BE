using DTOModel.UserDTO;

namespace DTOModel.TeamDTO
{
	public class GetTeamUpdate
	{
		public Guid Id { get; set; }
		public string name { get; set; }
		public List<UserPartDTO> users { get; set; }
	}
}
