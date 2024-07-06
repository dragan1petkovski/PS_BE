using DataAccessLayerDB;
using DomainModel.DB;
using DTOModel.TeamDTO;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Text.Json;

namespace be.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TeamController : ControllerBase
    {
        private readonly PSDBContext _dbContext;

        public TeamController(PSDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet("{userid}")]
        public IEnumerable<ClientTeamMapping> GetAllClientTeamMappingsByUserId(Guid userid)
        {
            TeamServicecs teamService = new TeamServicecs(_dbContext);
            return teamService.GetAllClientTeamMappingsByUserId(userid);
        }

        [HttpGet]
        public IEnumerable<TeamDTO> GetAllTeams()
        {
            TeamServicecs teamService = new TeamServicecs(_dbContext);
            return teamService.GetAllTeams();
        }


        [HttpPost]
        public string AddTeam(PostTeamDTOcs _newTeam)
        {
            TeamServicecs teamService = new TeamServicecs(_dbContext);
            return JsonSerializer.Serialize(teamService.AddTeam(_newTeam));

        }
        [HttpGet]
        public IEnumerable<ClientTeamMapping> GetAllClientTeamMappings()
        {
            TeamServicecs teamServicecs = new TeamServicecs (_dbContext);
            return teamServicecs.GetAllClientTeamMappings();
        }
    }
}
