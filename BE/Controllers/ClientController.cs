using Microsoft.AspNetCore.Mvc;
using Services;
using DataAccessLayerDB;
using DTOModel.ClientDTO;
using DomainModel;
using System.Text.Json;
using DataMapper;
using Microsoft.AspNetCore.Authorization;
using AuthenticationLayer;
using Microsoft.AspNetCore.Identity;
using DTOModel;

namespace be.Controllers
{
    [ApiController]
    [ResponseCache(NoStore = true)]
    public class ClientController : ControllerBase
    {
        private readonly PSDBContext _dbContext;
        private readonly ClientService _service;
        private readonly ClientDataMapper _dataMapper;
        private readonly UserAuthorization _userAuthorization;
        private readonly JwtTokenManager _jwtTokenManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ClientController(PSDBContext configuration, ClientService clientService, ClientDataMapper clientDataMapper, UserAuthorization userAuthorization, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, JwtTokenManager jwtTokenManager)
        {
            _dbContext = configuration;
            _service = clientService;
            _dataMapper = clientDataMapper;
            _userAuthorization = userAuthorization;
            _jwtTokenManager = jwtTokenManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "User")]
        [HttpGet("api/[controller]/[action]")]
        public IEnumerable<ClientDTOForUsers> GetCredentialClientsByUserId([FromServices] JwtTokenManager _jwtTokenManager)
        {
            return _service.GetCredentialClientsByUserId(_dbContext,_dataMapper, _jwtTokenManager, Request.Headers.Authorization);
        }


        [HttpGet("api/[controller]/[action]")]
        public IEnumerable<ClientDTOForUsers> GetCertificateClientsByUserId([FromServices] JwtTokenManager _jwtTokenManager)
        {
            return _service.GetCertificateClientsByUserId(_dbContext,_dataMapper,_jwtTokenManager, Request.Headers.Authorization);
        }


		[Authorize(Roles = "Administrator")]
		[HttpGet("api/[controller]/[action]")]
        public IEnumerable<ClientDTOForAdmins> GetAllFullClients()
        {
            return _dataMapper.ConvertClientListToClientDTOListForAdmins(_service.GetAllClients(_dbContext));
        }


		[Authorize(Roles = "Administrator")]
		[HttpGet("api/[controller]/[action]")]
        public IEnumerable<ClientDTOForUsers> GetAllPartClients()
        {
            return _dataMapper.ConvertClientListToClientDTOListForUsers(_service.GetAllClients(_dbContext));
        }


		[Authorize(Roles = "Administrator")]
		[HttpPost("api/[controller]/[action]")]
        public string Create(PostClient postClient)
        {
            
            return JsonSerializer.Serialize(_service.AddNewClient(_dbContext, postClient));

        }


		[Authorize(Roles = "Administrator")]
        [HttpGet("api/[controller]/[action]/{id:guid}")]
		public ClientUpdate Update(Guid id)
        {
            return _service.Update(id, _dbContext);
        }


        [Authorize(Roles ="Administrator")]
        [HttpPost("api/[controller]/[action]")]
        public async Task<IActionResult> Update(ClientUpdate update, [FromServices] ILogger<UserAuthorization> _logger)
        {
            if(await _userAuthorization.IsValidLoggedUser(_userManager,_roleManager,_jwtTokenManager,Request.Headers.Authorization,_logger))
            {
                return StatusCode(200,_service.Update(update, _dbContext));
            }
            return StatusCode(404);
        }

		[Authorize(Roles = "Administrator")]
        [HttpDelete("api/[controller]/[action]")]
        public async Task<IActionResult> Delete(DeleteAdminItem deleteitem, [FromServices] TeamService _teamService, [FromServices] CertificateService _certificateService, [FromServices] IConfiguration _configuration, [FromServices] EmailNotificationService _emailNotificationService, [FromServices] ILogger<UserAuthorization> _logger)
        {
            if(await _userAuthorization.IsValidLoggedUser(_userManager,_roleManager,_jwtTokenManager,Request.Headers.Authorization, _logger))
            {
				if (_emailNotificationService.TryValidateDeleteCode(_dbContext, deleteitem, out Guid _deleteVerificationId))
				{
					DeleteVerification deleteVerification = _dbContext.deleteVerifications.Find(_deleteVerificationId);
					if (deleteVerification != null)
					{
						_dbContext.deleteVerifications.Remove(deleteVerification);
                        _dbContext.SaveChanges();
						return StatusCode(200, _service.Delete(deleteitem.id, _teamService, _certificateService, _dbContext, _configuration));
					}
					return StatusCode(404);
				}
				return StatusCode(404);
			}
            return StatusCode(404);
        }
    }
}
