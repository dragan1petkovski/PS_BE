using DTOModel.TeamDTO;

namespace DTOModel.UserDTO
{
	public class PostUpdateUser
	{
		public Guid id {  get; set; }
		public string firstname { get; set; }
		public string lastname { get; set; }
		public string email { get; set; }
		public string username { get; set; }

		public List<ClientTeamPair> clientTeamPairs { get; set; }
	}
}

