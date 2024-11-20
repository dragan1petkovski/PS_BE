using DataAccessLayerDB;
using DataMapper;
using DTO.Credential;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AppServices;
using System.Text.Json;
using DomainModel;
using EncryptionLayer;
using Microsoft.AspNetCore.Authorization;
using LogginMessages;
using AAAService.Core;
using AAAService.Validators;
using DTO.Personal;
using Microsoft.AspNetCore.SignalR;
using SignalR;
using Newtonsoft.Json;
using DTO.Team;


namespace BE.Controllers
{
    [ApiController]
    [Authorize(Roles = "User")]
    
    [ResponseCache(NoStore = true)]
    public class CredentialController : ControllerBase
    {
        private readonly PSDBContext _dbContext;
        private readonly CredentialService _credentialService;
        private readonly JwtManager _jwtManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<CredentialService> _logger;
		private readonly IHubContext<DataSync> _hubContext;
		public CredentialController(PSDBContext dbContext, CredentialService credentialService, JwtManager jwtTokenManager, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ILogger<CredentialService> logger, IHubContext<DataSync> hubContext)
        {
            _dbContext = dbContext;
            _credentialService = credentialService;
            _jwtManager = jwtTokenManager;
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
			_hubContext = hubContext;
        }


        [HttpGet("api/[controller]")]
		public async Task<IActionResult> GetCredentialByClientID([FromQuery]Guid? tid,[FromQuery]Guid? cid,[FromQuery]Guid? credid, [FromServices] CredentialDataMapper dataMapper, [FromServices] Validation validation)
        {
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(StatusMessages.AccessDenied, StatusMessages.AccessDenied);
			}

			(bool _, Guid userid) = _jwtManager.GetUserID(Request.Headers.Authorization);
			if(!credid.HasValue && !tid.HasValue && cid.HasValue)
			{
				(StatusMessages status, List<DTO.Credential.Credential> output) = _credentialService.GetClientCredentialsByClientId(userid, cid.HasValue?cid.Value:Guid.Empty, _dbContext, dataMapper);
				if(StatusMessages.Ok == status)
				{
					return StatusCode(status,output);
				}
				return StatusCode(status, status);
			}
			else if(tid.HasValue && credid.HasValue)
			{
				(StatusMessages status, DTO.Credential.Credential output) = _credentialService.GetClientCredentialByCredentialId(userid, tid.HasValue?tid.Value:Guid.Empty, credid.HasValue?credid.Value:Guid.Empty, _dbContext, dataMapper);
				if (StatusMessages.Ok == status)
				{
					return StatusCode(status, output);
				}
				return StatusCode(status, status);
			}
			else
			{
				return StatusCode(StatusMessages.ResourceNotFound, StatusMessages.ResourceNotFound);
			}
        }

		[HttpGet("api/[controller]/personal/")]
		public async Task<IActionResult> GetPersonalCredentialById([FromQuery] Guid? pfid, [FromQuery]Guid? credid, [FromServices] CredentialDataMapper _dataMapper, [FromServices] Validation validation)
		{

			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(StatusMessages.AccessDenied, StatusMessages.AccessDenied);
			}
			(bool _, Guid _userid) = _jwtManager.GetUserID(Request.Headers.Authorization);
			if(!credid.HasValue && pfid.HasValue)
			{
				(StatusMessages status, List<PersonalCredential> output) = _credentialService.GetPersonalCredentialsByFolderId(_userid, pfid.HasValue?pfid.Value:Guid.Empty , _dbContext);
				if (StatusMessages.Ok == status)
				{
					return StatusCode(status, output);
				}
				return StatusCode(status, status);
			}
            else if(!pfid.HasValue && !credid.HasValue)
            {
				(StatusMessages status, List<PersonalCredential> output) = _credentialService.GetPersonalCredentialsByUserId(_userid,_dbContext);
				if (StatusMessages.Ok == status)
				{
					return StatusCode(status, output);
				}
				return StatusCode(status, status);
			}
			else if(!pfid.HasValue && credid.HasValue)
			{
				(StatusMessages status, PersonalCredential output) = _credentialService.GetPersonalCredentialByCredentialId(_userid,credid.HasValue?credid.Value:Guid.Empty, _dbContext);
				if (StatusMessages.Ok == status)
				{
					return StatusCode(status, output);
				}
				return StatusCode(status, status);
			}
			else
			{
				(StatusMessages status, PersonalCredential output) = _credentialService.GetPersonalCredentialByCredentialId(_userid, pfid.HasValue ? pfid.Value : Guid.Empty, credid.HasValue ? credid.Value : Guid.Empty, _dbContext);
				if (StatusMessages.Ok == status)
				{
					return StatusCode(status, output);
				}
				return StatusCode(status, status);
			}



		}

		[HttpPost("api/[controller]")]
		public async Task<IActionResult> SetCredential(PostCredential credential, [FromServices] SymmetricEncryption symmetricEncryption, [FromServices] IConfiguration configuration, [FromServices] Validation validation)
        {
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(StatusMessages.AccessDenied, StatusMessages.AccessDenied);
			}
			(bool _, Guid _userid) = _jwtManager.GetUserID(Request.Headers.Authorization);
			(StatusMessages status, List<DTO.Credential.Credential> data) = _credentialService.CreateCredential(_userid, credential, _dbContext, symmetricEncryption, configuration, _logger);
			if(StatusMessages.AddNewCredential == status)
			{
				foreach(var i in data)
				{
					_hubContext.Clients.Group(JsonConvert.SerializeObject(new { id = i.teamid, type = "credential" }))
										.SendAsync("Notification",JsonConvert.SerializeObject(new { status = "new", type = "credential", data = i }));
				}
				
			}
			return StatusCode(status, status);

		}

		[HttpPost("api/[controller]/personal")]
		public async Task<IActionResult> AddPersonalCredential(PostPersonalCredential postPersonalCredential, [FromServices] SymmetricEncryption _symmetricEncryption, [FromServices] IConfiguration _configuration, [FromServices] Validation _validation)
		{
			_validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			if (!(await _validation.ProcessAsync()))
			{
				return StatusCode(StatusMessages.AccessDenied, StatusMessages.AccessDenied);
			}
			(_, Guid userid) = _jwtManager.GetUserID(Request.Headers.Authorization);
			(StatusMessages status,PersonalCredential output) = _credentialService.CreatePersonalCredential(userid, postPersonalCredential, _dbContext, _symmetricEncryption, _configuration);
			if(status == StatusMessages.AddNewCredential)
			{
				_hubContext.Clients.Group(JsonConvert.SerializeObject(new { id = new List<Guid>() { output.personalfolderid.HasValue ? output.personalfolderid.Value : userid }, type = "personal" }))
										.SendAsync("Notification",JsonConvert.SerializeObject(new {status="new",type="personal",data=output}));
			}
			return StatusCode(status, status);
		}

		[HttpDelete("api/[controller]/{id:guid}/{teamid:guid}")]
		public async Task<IActionResult> Delete( Guid id,Guid teamid, [FromServices] Validation validation)
        {
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
            if (!(await validation.ProcessAsync()))
            {
				return StatusCode(StatusMessages.AccessDenied, StatusMessages.AccessDenied);
			}
            (bool _, Guid _userid) = _jwtManager.GetUserID(Request.Headers.Authorization);

			StatusMessages status = _credentialService.DeleteCredential(id, teamid, _userid, _dbContext, _logger);
			if(StatusMessages.DeleteCredential == status)
			{
				_hubContext.Clients.Group(JsonConvert.SerializeObject(new { id = teamid, type = "credential" }))
									.SendAsync("Notification", JsonConvert.SerializeObject(new { status = "delete", type = "credential", data = id }));
			}
			return StatusCode(status, status);
		}

        [HttpDelete("api/[controller]/personal/{id:guid}/{personalFolderId:guid?}")]
		public async Task<IActionResult> DeletePersonal(Guid id, Guid? personalFolderId, [FromServices] Validation validation)
        {
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
            if (!(await validation.ProcessAsync()))
            {
				return StatusCode(StatusMessages.AccessDenied, StatusMessages.AccessDenied);
			}
            (bool _, Guid _userid) = _jwtManager.GetUserID(Request.Headers.Authorization);
			StatusMessages status = _credentialService.DeletePersonalCredential(id, personalFolderId.HasValue?personalFolderId.Value:Guid.Empty, _userid, _dbContext, _logger);
			if (StatusMessages.DeleteCredential == status)
			{
				_hubContext.Clients.Group(JsonConvert.SerializeObject(new { id = new List<Guid>() { personalFolderId.HasValue ? personalFolderId.Value : _userid }, type = "personal" }))
									.SendAsync("Notification", JsonConvert.SerializeObject(new { status = "delete", type = "personal", data = id }));
			}
			return StatusCode(status, status);
		}

        [HttpPut("api/[controller]")]
        public async Task<IActionResult> Update(PostUpdateCredential update, [FromServices] IConfiguration _configuration, [FromServices] SymmetricEncryption _symmetricEncryption, [FromServices] Validation validation)
        {
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(StatusMessages.AccessDenied, StatusMessages.AccessDenied);
			}
			(bool _, Guid _userid) = _jwtManager.GetUserID(Request.Headers.Authorization);
			(StatusMessages status, DTO.Credential.Credential cred) = _credentialService.Update(_dbContext, _userid, update, _configuration, _symmetricEncryption);
			if(StatusMessages.UpdateCredential == status)
			{
				_hubContext.Clients.Group(JsonConvert.SerializeObject(new { id = cred.teamid, type = "credential" }))
									.SendAsync("Notification", JsonConvert.SerializeObject(new { status = "update", type = "credential", data = cred }));
			}
			return StatusCode(status, status);
		}

        [HttpPut("api/[controller]/personal")]
        public async Task<IActionResult> UpdatePersonal([FromBody] PostUpdatePersonalCredential update, [FromServices] IConfiguration _configuration, [FromServices] SymmetricEncryption _symmetricEncryption, [FromServices] Validation validation)
		{
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			if(!(await validation.ProcessAsync()))
            {
				return StatusCode(StatusMessages.AccessDenied, StatusMessages.AccessDenied);
			}
            (bool _, Guid _userid) = _jwtManager.GetUserID(Request.Headers.Authorization);
			Console.WriteLine(JsonConvert.SerializeObject(update));
			(StatusMessages status, PersonalCredential cred) = _credentialService.UpdatePersonalCredential(_dbContext, update, _configuration, _symmetricEncryption, _userid);
			if(status == StatusMessages.UpdateCredential)
			{
				if(string.IsNullOrEmpty(update.originalpersonalfolderid) || string.IsNullOrWhiteSpace(update.originalpersonalfolderid))
				{
					if(cred.personalfolderid == null)
					{
						_hubContext.Clients.Group(JsonConvert.SerializeObject(new { id = new List<string>() { _userid.ToString() }, type = "personal" })).SendAsync("Notification",JsonConvert.SerializeObject(new { status = "update", type = "personal", data = cred }));
					}
					else
					{
						_hubContext.Clients.Group(JsonConvert.SerializeObject(new { id = new List<string>() { _userid.ToString() }, type = "personal" })).SendAsync("Notification", JsonConvert.SerializeObject(new { status = "delete", type = "personal", data = cred.id }));
					}
				}
				else
				{
					if(cred.personalfolderid.HasValue && (!string.IsNullOrEmpty(update.originalpersonalfolderid) || !string.IsNullOrWhiteSpace(update.originalpersonalfolderid)))
					{
						if(cred.personalfolderid.ToString() == update.originalpersonalfolderid)
						{
							_hubContext.Clients.Group(JsonConvert.SerializeObject(new { id = new List<string>() { update.originalpersonalfolderid }, type = "personal" })).SendAsync("Notification", JsonConvert.SerializeObject(new { status = "update", type = "personal", data = cred }));
						}
						else
						{
							_hubContext.Clients.Group(JsonConvert.SerializeObject(new { id = new List<string>() { update.originalpersonalfolderid }, type = "personal" })).SendAsync("Notification", JsonConvert.SerializeObject(new { status = "delete", type = "personal", data = cred.id }));
						}
						
					}
					else
					{
						_hubContext.Clients.Group(JsonConvert.SerializeObject(new { id = new List<string>() { update.originalpersonalfolderid }, type = "personal" })).SendAsync("Notification", JsonConvert.SerializeObject(new { status = "delete", type = "personal", data = cred.id }));
					}
					
				}
			}
			return StatusCode(status, status);

		}

		[HttpPost("api/[controller]/[action]")]
		public async Task<IActionResult> GiveCredential(PostGiveCredential giveCredential, [FromServices] SymmetricEncryption _symmetricEncryption, [FromServices] IConfiguration _configuration, [FromServices] Validation validation)
		{
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(StatusMessages.AccessDenied, StatusMessages.AccessDenied);
			}
			StatusMessages status = _credentialService.GiveCredential(giveCredential, _dbContext, _configuration, _symmetricEncryption);
			return StatusCode(status, status);
		}
	}
}
