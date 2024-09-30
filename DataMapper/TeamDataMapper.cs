using TransitionObjectMapper;
using DomainModel;
using DTOModel.TeamDTO;

namespace DataMapper
{
	public class TeamDataMapper
	{
		public List<ClientTeamMapping> ConvertToClientTeamMappingList(List<Team> teams)
		{
			return teams.Select(t => new ClientTeamMapping()
			{
				clientid = t.client.id,
				clientname = t.client.name,
				teamid = t.id,
				teamname = t.name,
			}).ToList();

		}

		public List<TeamDTO> ConvertTeamListToDTOList(List<Team> teams)
		{
			return teams.Select(t => new TeamDTO()
			{
				id = t.id,
				clientid = t.client.id,
				clientname = t.client.name,
				name = t.name,
				createdate = t.createdate,
				updatedate = t.updatedate
			}).ToList();

		}

	}
}
