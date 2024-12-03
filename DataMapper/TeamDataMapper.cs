using TransitionObjectMapper;
using DomainModel;
using DTO.Team;

namespace DataMapper
{
	public class TeamDataMapper
	{
		public List<ClientTeamMapping> ConvertToClientTeamMapping(List<DomainModel.Team> teams)
		{
			return teams.Select(t => new ClientTeamMapping()
			{
				clientid = t.client.id,
				clientname = t.client.name,
				teamid = t.id,
				teamname = t.name,
			}).ToList();

		}

		public List<DTO.Team.Team> ConvertToDTO(List<DomainModel.Team> teams)
		{
			return teams.Select(t => new DTO.Team.Team()
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
