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
    
        public List<TeamDTOMini> GetAllClientTeamMappingsByUserId(Guid userid) 
        {
            List<TeamDTOMini> output = new List<TeamDTOMini>();

            //User splitQuery() to increase performance
            var listofteams = _dbContext.Users.Include(u => u.teams)
                            .Include(u => u.teams).ThenInclude(t => t.client)
                            .Where(u => u.id == userid).First()
                            .teams.Select(t => new TeamDTOMini() { teamid = t.id, clientid = t.client.id, teamname = t.name, clientname = t.client.name }).ToList();
            
            foreach( var team in listofteams )
            {
                  output.Add(team);
            }
            
            return output;
        }

        public List<TeamDTOMini> GetAllClientTeamMappings()
        {
            List<TeamDTOMini> result = new List<TeamDTOMini>();

            _dbContext.Teams.Include(t => t.client).Select(t => new TeamDTOMini()
            {
                teamid = t.id,
                teamname = t.name,
                clientid = t.client.id,

            });

            return result;
        }

        private List<TeamDTOMini> ConvertToTeamDTOMini(List<TeamDBDM> teams)
        {
            return new List<TeamDTOMini>();
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
