using DTOModel.CredentialDTO;
using DTOModel.PersonalDTO;
using Microsoft.AspNetCore.Mvc;
using Services;
using DataAccessLayerDB;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using AuthenticationLayer;
using DTOModel;
using EncryptionLayer;
using Microsoft.AspNetCore.Identity;
using DomainModel;

namespace be.Controllers
{
    [ApiController]
	[Authorize(Roles = "User")]
	[ResponseCache(NoStore = true)]
    public class PersonalController : ControllerBase
    {
        private readonly PSDBContext _dbContext;
        private readonly JwtTokenManager _jwtTokenManager;
        private readonly PersonalService _service;

        public PersonalController(PSDBContext dbContext, JwtTokenManager jwtTokenManager, [FromServices] PersonalService personalService)
        {
            _dbContext = dbContext;
            _jwtTokenManager = jwtTokenManager;
			_service = personalService;
        }

        [HttpGet("api/[controller]/[action]/{folderid:guid?}")]
        public IEnumerable<PersonalCredentialDTO> GetCredentialsByFoderId(Guid folderid, [FromServices] CredentialService credentialService)
        {
            return credentialService.GetCredentialsByFolderID(folderid, _dbContext, _jwtTokenManager, Request.Headers.Authorization);
            //return new List<CredentialDTO>();
        }

        [HttpGet("api/[controller]/[action]")]
        public IEnumerable<PersonalFolderDTO> GetCredentialFoldersByUserID()
        {
            return _service.GetPersonalFoldersByUserid(_dbContext,_jwtTokenManager,Request.Headers.Authorization);
        }
        
        
        [HttpPost("api/[controller]/[action]")]
        public SetStatus AddFolderByUserId(PostPersonalFolderDTO FolderName)
        {
            return _service.AddPersonalFolder(FolderName.name, _dbContext,_jwtTokenManager, Request.Headers.Authorization);
        }

        [HttpPost("api/[controller]/[action]")]
        public string AddCredentialByUserId(PostPersonalCredentialDTO postPersonalCredential, [FromServices] SymmetricEncryption _symmetricEncryption, [FromServices] IConfiguration _configuration)
        {
            return JsonSerializer.Serialize(_service.AddPersonalCredential(postPersonalCredential, _dbContext, _jwtTokenManager, Request.Headers.Authorization, _symmetricEncryption,_configuration));
        }

        [HttpDelete("api/[controller]/[action]/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, [FromServices] UserAuthorization _userAuthorization, [FromServices] UserManager<User> _userManager, [FromServices] RoleManager<IdentityRole> _roleManager, [FromServices] ILogger<UserAuthorization> _logger)
        {
            if(await _userAuthorization.IsValidLoggedUser(_userManager,_roleManager,_jwtTokenManager,Request.Headers.Authorization, _logger))
            {
                return StatusCode(200, _service.Delete(id, _dbContext,_jwtTokenManager, Request.Headers.Authorization));

			}
            return StatusCode(404);
        }

		[HttpDelete("api/[controller]/[action]")]
		public async Task<IActionResult> Delete(PersonalFolderList ids, [FromServices] UserAuthorization _userAuthorization, [FromServices] UserManager<User> _userManager, [FromServices] RoleManager<IdentityRole> _roleManager, [FromServices] ILogger<UserAuthorization> _logger)
		{
			if (await _userAuthorization.IsValidLoggedUser(_userManager, _roleManager, _jwtTokenManager, Request.Headers.Authorization, _logger))
			{
				return StatusCode(200, _service.Delete(ids, _dbContext, _jwtTokenManager, Request.Headers.Authorization));

			}
			return StatusCode(404);
		}
	
    }
}
