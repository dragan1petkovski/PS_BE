using DTOModel.TeamDTO;
using DomainModel.DB;
using DataAccessLayerDB;
using Microsoft.EntityFrameworkCore;
using DTOModel;


namespace Services
{
    public class TeamServicecs
    {
        private readonly PSDBContext _dbContext;

        public TeamServicecs(PSDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<TeamDTO> GetAllTeams()
        {
            _dbContext.Teams.Include(t => t.client).ToList();
            return ConvertTeamDBDMListToDTOList(_dbContext.Teams.Include(t => t.client).ToList());
        }
    
        public List<ClientTeamMapping> GetAllClientTeamMappingsByUserId(Guid userid) 
        {
            List<ClientTeamMapping> output = new List<ClientTeamMapping>();

            //User splitQuery() to increase performance
            var listofteams = _dbContext.Users.Include(u => u.teams)
                            .Include(u => u.teams).ThenInclude(t => t.client)
                            .Where(u => u.id == userid).First()
                            .teams.Select(t => new ClientTeamMapping() { teamid = t.id, clientid = t.client.id, teamname = t.name, clientname = t.client.name }).ToList();
            
            foreach( var team in listofteams )
            {
                  output.Add(team);
            }
            
            return output;
        }

        public List<ClientTeamMapping> GetAllClientTeamMappings()
        {
            return _dbContext.Teams.Include(t => t.client).Select(t => new ClientTeamMapping(){
                teamname = t.name,
                teamid = t.id,
                clientid = t.client.id,
                clientname = t.client.name,
            }).ToList();
        }

        private List<ClientTeamMapping> ConvertToTeamDTOMini(List<TeamDBDM> teams)
        {
            return new List<ClientTeamMapping>();
        }

        private List<TeamDTO> ConvertTeamDBDMListToDTOList(List<TeamDBDM> teams)
        {
            List<TeamDTO> result = new List<TeamDTO> ();
            foreach( TeamDBDM team in teams )
            {
                    result.Add(new TeamDTO()
                    {
                        id = team.id,
                        clientname = team.client.name,
                        name = team.name,
                        createdate = team.createdate,
                        updatedate = team.updatedate
                    });

            }
            return result;
        }
    
        public SetStatus AddTeam(PostTeamDTOcs _newTeam)
        {
            try
            {
                TeamDBDM newTeam = new TeamDBDM();
                newTeam.id = Guid.NewGuid();
                newTeam.name = _newTeam.name;
                newTeam.client = _dbContext.Clients.Find(_newTeam.clientid);
                newTeam.users = _dbContext.Users.Where(u => _newTeam.userids.Any(tu => tu == u.id)).ToList();
                newTeam.updatedate = DateTime.Now;
                newTeam.createdate = DateTime.Now;

                _dbContext.Teams.Add(newTeam);
                _dbContext.SaveChanges();
                Console.WriteLine(newTeam.name);
                return new SetStatus() { status = "OK" };
            }
            catch
            {
                return new SetStatus() { status = "KO" };
            }

        }
    }
}
