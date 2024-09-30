using AuthenticationLayer;
using DataAccessLayerDB;
using DataMapper;
using DomainModel;
using DTOModel;
using DTOModel.TeamDTO;
using DTOModel.UserDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Text.Json;

namespace be.Controllers
{
    [ApiController]
	[ResponseCache(NoStore = true)]
    public class TeamController : ControllerBase
    {
        private readonly PSDBContext _dbContext;
        private readonly TeamService _service;
        private readonly TeamDataMapper _dataMapper;
        private readonly JwtTokenManager _jwtTokenManager;
        private readonly UserAuthorization _userAuthorization;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public TeamController(PSDBContext dbContext, TeamService teamService, TeamDataMapper teamDataMapper, JwtTokenManager jwtTokenManager, UserAuthorization userAuthorization, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
			_service = teamService;
            _dataMapper = teamDataMapper;
            _jwtTokenManager = jwtTokenManager;
            _userAuthorization = userAuthorization;
            _userManager = userManager;
            _roleManager = roleManager;

        }
        
        
        [HttpGet("api/[controller]/[action]")]
		[Authorize(Roles = "User, Administrator")]
		public IEnumerable<ClientTeamMapping> GetAllClientTeamMappingsByUserId([FromServices] JwtTokenManager _jwtTokenManager)
        {
            return _service.GetAllClientTeamMappingsByUserId(_dbContext,_dataMapper,_jwtTokenManager, Request.Headers.Authorization);
        }

        
        [HttpGet("api/[controller]/[action]")]
		[Authorize(Roles = "Administrator")]
		public IEnumerable<TeamDTO> GetAllTeams()
        {
            return _service.GetAllTeams(_dbContext,_dataMapper);
        }

        
        [HttpPost("api/[controller]/[action]")]
		[Authorize(Roles = "Administrator")]
		public string Create(PostTeamDTO _newTeam)
        {
            return JsonSerializer.Serialize(_service.Create(_newTeam,_dbContext));

        }
        
        
        [HttpGet("api/[controller]/[action]")]
		[Authorize(Roles = "Administrator, User")]
		public IEnumerable<ClientTeamMapping> GetAllClientTeamMappings()
        {
            return _service.GetAllClientTeamMappings(_dbContext);
        }


        [HttpPost("api/[controller]/[action]")]
        [Authorize(Roles ="Administrator")]
        public async Task<IActionResult> Update(PostTeamUpdate update, [FromServices] ILogger<UserAuthorization> _logger)
        {
            if(await _userAuthorization.IsValidLoggedUser(_userManager,_roleManager,_jwtTokenManager,Request.Headers.Authorization, _logger))
            {
				return StatusCode(200, _service.Update(update, _dbContext));
			}
			return StatusCode(404);

		}


		[HttpGet("api/[controller]/[action]/{id:guid}")]
		public async Task<GetTeamUpdate> Update(Guid id, [FromServices] RoleManager<IdentityRole> _roleManager, [FromServices] UserDataMapper _userDataMapper, [FromServices] ILogger<UserAuthorization> _logger)
		{
			if (await _userAuthorization.IsValidLoggedUser(_userManager, _roleManager, _jwtTokenManager, Request.Headers.Authorization,_logger))
			{
				return await _service.Update(_dbContext, id, _userDataMapper, _userManager);
			}

			return new GetTeamUpdate();
		}


		[HttpDelete("api/[controller]/[action]/")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(DeleteAdminItem deleteitem, [FromServices] IConfiguration _configuration, [FromServices] CertificateService _certificateService, [FromServices] EmailNotificationService _emailNotificationService, [FromServices] ILogger<UserAuthorization> _logger)
        {
            if(await _userAuthorization.IsValidLoggedUser(_userManager,_roleManager, _jwtTokenManager, Request.Headers.Authorization, _logger))
            {
				if (_emailNotificationService.TryValidateDeleteCode(_dbContext, deleteitem, out Guid _deleteVerificationId))
				{
					DeleteVerification deleteVerification = _dbContext.deleteVerifications.Find(_deleteVerificationId);
					if (deleteVerification != null)
					{
						_dbContext.deleteVerifications.Remove(deleteVerification);
                        _dbContext.SaveChanges();
						return StatusCode(200, _service.Delete(deleteitem.id, _certificateService, _dbContext, _configuration));
					}
					return StatusCode(404);
				}
				return StatusCode(404);
			}
            return StatusCode(404);
        }
    }
}
