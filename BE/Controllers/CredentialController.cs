using DataAccessLayerDB;
using DataMapper;
using DTOModel.CredentialDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Text.Json;
using DomainModel;
using EncryptionLayer;
using Microsoft.AspNetCore.Authorization;
using AuthenticationLayer;
using DTOModel;
using Microsoft.EntityFrameworkCore;
using LogginMessages;


namespace be.Controllers
{
    [ApiController]
    [Authorize(Roles = "User")]
    [Route("api/[controller]/[action]")]
    [ResponseCache(NoStore = true)]
    public class CredentialController : ControllerBase
    {
        private readonly PSDBContext _dbContext;
        private readonly CredentialService _credentialService;
        private readonly JwtTokenManager _jwtTokenManager;
        private readonly UserAuthorization _userAuthorization;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CredentialController(PSDBContext dbContext, CredentialService credentialService, JwtTokenManager jwtTokenManager, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, UserAuthorization userAuthorization)
        {
            _dbContext = dbContext;
            _credentialService = credentialService;
            _jwtTokenManager = jwtTokenManager;
            _userAuthorization = userAuthorization;
            _roleManager = roleManager;
            _userManager = userManager;
        }


        [HttpGet("{clientid:guid}")]
        public IEnumerable<CredentialDTO> GetCredentialByClientID(Guid clientid, [FromServices] CredentialDataMapper dataMapper)
        {

            return _credentialService.GetCredentialsByClintId(clientid, dataMapper, _dbContext, _jwtTokenManager, Request.Headers.Authorization);
        }

        [HttpPost]
        public string SetCredential(PostCredentialDTO credential, [FromServices] SymmetricEncryption symmetricEncryption, [FromServices] IConfiguration configuration)
        {

            return JsonSerializer.Serialize(_credentialService.AddCredential(credential, _dbContext, symmetricEncryption, configuration));
            //return "nesto";
        }

        [HttpDelete()]
        public SetStatus Delete(DeleteItem deleteItem, ILogger<CredentialService> _logger)
        {
            try
            {
                return _credentialService.DeleteCredential(deleteItem, _dbContext, _jwtTokenManager, Request.Headers.Authorization, _logger);
            }
            catch
            {
                return new SetStatus() { status = "KO", errorMessage = "Credential can't be deleted at the moment try again later" };
            }
        }

        [HttpDelete()]
        public SetStatus DeletePersonal(PersonalCredentialId deleteItem, ILogger<CredentialService> _logger)
        {
            try
            {
                return _credentialService.DeletePersonalCredential(deleteItem, _dbContext, _jwtTokenManager, Request.Headers.Authorization, _logger);
            }
            catch
            {
                return new SetStatus() { status = "KO", errorMessage = "Credential can't be deleted at the moment try again later" };
            }
        }

        [HttpPost]
        public IActionResult GiveCredential(PostGiveCredentialDTO giveCredential, [FromServices] SymmetricEncryption _symmetricEncryption, [FromServices] IConfiguration _configuration)
        {
            return StatusCode(200, _credentialService.GiveCredential(giveCredential, _dbContext, _jwtTokenManager, _configuration, _symmetricEncryption, Request.Headers.Authorization));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCredentialById(Guid id, [FromServices] CredentialDataMapper _dataMapper, [FromServices] ILogger<UserAuthorization> _logger)
        {
            if (await _userAuthorization.IsValidLoggedUser(_userManager, _roleManager, _jwtTokenManager, Request.Headers.Authorization, _logger))
            {
                Credential credential = _dbContext.Credentials.Find(id);
                if (credential != null)
                {
                    return StatusCode(200, _dataMapper.ConvertToCredentialDTO(credential));
                }
                return StatusCode(404);
            }
            return StatusCode(401);
        }

        [HttpPost]
        public async Task<IActionResult> GetPersonalCredentialById(PersonalCredentialId personalCredential, [FromServices] CredentialDataMapper _dataMapper, [FromServices] ILogger<UserAuthorization> _logger)
        {
            if (await _userAuthorization.IsValidLoggedUser(_userManager, _roleManager, _jwtTokenManager, Request.Headers.Authorization, _logger))
            {
                if (_jwtTokenManager.GetUserID(Request.Headers.Authorization, out Guid _userid))
                {
                    if (Guid.TryParse(personalCredential.personalfolderid,out Guid _personalfolderid))
                    {
                        User user = new User();
                        try
                        {
                            user = _dbContext.Users.Include(u => u.folders)
                                                   .Include(u => u.folders).ThenInclude(pf => pf.credentials)
                                                   .FirstOrDefault(u => u.Id == _userid.ToString());
                            if (user != null)
                            {
                                PersonalFolder personalFolder = new PersonalFolder();
                                personalFolder = user.folders.FirstOrDefault(pf => pf.id == _personalfolderid);
                                if (personalFolder != null)
                                {
                                    Credential credential = new Credential();
                                    credential = personalFolder.credentials.FirstOrDefault(c => c.id == personalCredential.id);
                                    if (credential != null)
                                    {
                                        PersonalCredentialDTO personalCredentialDTO = new PersonalCredentialDTO();
                                        personalCredentialDTO = _dataMapper.ConvertToPersonalCredentialDTO(credential);
                                        personalCredentialDTO.personalfolderid = _personalfolderid;
                                        return StatusCode(200, personalCredentialDTO);
                                    }
                                    return StatusCode(404);
                                }
                                return StatusCode(404);
                            }
                            return StatusCode(404);

                        }
                        catch (Exception ex)
                        {

                            _logger.LogError(DatabaseLog.DBConnectionLog(ex.Message));
                            return StatusCode(404);
                        }
                    }
                    else
                    {
                        Credential credential = _dbContext.Credentials.Find(personalCredential.id);
                        if (credential != null)
                        {
                            return StatusCode(200, _dataMapper.ConvertToPersonalCredentialDTO(credential));
                        }
                        return StatusCode(404);
                    }
                }


            }
            return StatusCode(401);
        }


        [HttpPost]
        public async Task<IActionResult> Update(PostUpdateCredential update, [FromServices] IConfiguration _configuration, [FromServices] SymmetricEncryption _symmetricEncryption, [FromServices] ILogger<UserAuthorization> _logger)
        {
            if (await _userAuthorization.IsValidLoggedUser(_userManager, _roleManager, _jwtTokenManager, Request.Headers.Authorization, _logger))
            {
                return StatusCode(200, _credentialService.Update(_dbContext, update, _configuration, _symmetricEncryption));
            }
            return StatusCode(404);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePersonal([FromBody] PostUpdatePersonalCredential update, [FromServices] IConfiguration _configuration, [FromServices] SymmetricEncryption _symmetricEncryption, [FromServices] ILogger<UserAuthorization> _logger)
		{
			if (await _userAuthorization.IsValidLoggedUser(_userManager, _roleManager, _jwtTokenManager, Request.Headers.Authorization, _logger))
			{
                if(_jwtTokenManager.GetUserID(Request.Headers.Authorization, out Guid _userid))
                {
					return StatusCode(200, _credentialService.UpdatePersonalCredential(_dbContext, update, _configuration, _symmetricEncryption, _userid));
				}
			}
			return StatusCode(404);
		}
	}
}
