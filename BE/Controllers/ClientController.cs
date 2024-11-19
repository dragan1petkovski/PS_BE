using Microsoft.AspNetCore.Mvc;
using AppServices;
using DataAccessLayerDB;
using DTO.Client;
using DomainModel;
using System.Text.Json;
using DataMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using DTO;
using AAAService.Core;
using AAAService.Validators;
using LogginMessages;
using Microsoft.AspNetCore.SignalR;
using SignalR;
using Newtonsoft.Json;

namespace BE.Controllers
{
    [ApiController]
    [ResponseCache(NoStore = true)]
    public class ClientController : ControllerBase
    {
        private readonly PSDBContext _dbContext;
        private readonly ClientService _service;
        private readonly ClientDataMapper _dataMapper;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
		private readonly JwtManager _jwtManager;
		private readonly ILogger<ClientService> _logger;
		private readonly IHubContext<DataSync> _hubContext;
		public ClientController(PSDBContext configuration, ClientService clientService, ClientDataMapper clientDataMapper, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, [FromServices] JwtManager jwtManager, ILogger<ClientService> logger, IHubContext<DataSync> hubContext)
        {
            _dbContext = configuration;
            _service = clientService;
            _dataMapper = clientDataMapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtManager = jwtManager;
			_logger = logger;
			_hubContext = hubContext;
        }

        [Authorize(Roles = "User")]
        [HttpGet("api/[controller]/credential")]
        public async Task<IActionResult> GetCredentialClientsByUserId([FromServices] Validation validation)
        {
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(403,StatusMessages.AccessDenied);
			}
			(_, Guid userid) = _jwtManager.GetUserID(Request.Headers.Authorization);
			(StatusMessages status, List<ClientForUsers> output)= _service.GetCredentialClientsByUserId(_dbContext,_dataMapper,userid);
			if(status == StatusMessages.Ok)
			{
				return StatusCode(status, output);
			}
			return StatusCode(status, status);
		}


        [HttpGet("api/[controller]/certificate")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetCertificateClientsByUserId([FromServices] Validation validation)
        {
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(403,StatusMessages.AccessDenied);
			}
			(_, Guid userid) = _jwtManager.GetUserID(Request.Headers.Authorization);
			(StatusMessages status, List<ClientForUsers> output) = _service.GetCertificateClientsByUserId(_dbContext,_dataMapper, userid);
			if (status == StatusMessages.Ok)
			{
				return StatusCode(status, output);
			}
			return StatusCode(status, status);
		}


		[Authorize(Roles = "Administrator")]
		[HttpGet("api/[controller]/full/{id:guid?}")]
        public async Task<IActionResult> GetAllFullClients(Guid id, [FromServices] Validation validation)
        {
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			validation.AddValidator(new IsUserAdmin(Request.Headers.Authorization,_userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(403, StatusMessages.AccessDenied);
			}
			if (id == Guid.Empty)
            {
				(StatusMessages status, List<ClientForAdmins> output) = _service.GetAllClientsForAdmin(_dbContext,_dataMapper);
				if (status == StatusMessages.Ok)
				{
					return StatusCode(status, output);
				}
				return StatusCode(status, status);
			}
            else
            {
				(StatusMessages status, ClientUpdate output) = _service.Update(id, _dbContext);
				if (status == StatusMessages.Ok)
				{
					return StatusCode(status, output);
				}
				return StatusCode(status, status);
			}
        }


		[Authorize(Roles = "Administrator")]
		[HttpGet("api/[controller]/part")]
        public async Task<IActionResult> GetAllPartClients([FromServices] Validation validation)
        {
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			validation.AddValidator(new IsUserAdmin(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(403, StatusMessages.AccessDenied);
			}
			(StatusMessages status, List<ClientForUsers> output) = _service.GetAllClientsForUser(_dbContext,_dataMapper);
			if (status == StatusMessages.Ok)
			{
				return StatusCode(status, output);
			}
			return StatusCode(status, status);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost("api/[controller]")]
		public async Task<IActionResult> Create(PostClient postClient, [FromServices] Validation validation)
        {
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			validation.AddValidator(new IsUserAdmin(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(403, StatusMessages.AccessDenied);
			}
			(StatusMessages status,DTO.Client.ClientForAdmins output) = _service.Create(_dbContext, postClient);
			if(status == StatusMessages.AddNewClient)
			{
				_hubContext.Clients.Group("client").SendAsync("AdminNotification", JsonConvert.SerializeObject(new {status="new",type="client",data=output}));
			}
			return StatusCode(status,status);
        }

        [Authorize(Roles ="Administrator")]
        [HttpPut("api/[controller]")]
        public async Task<IActionResult> Update(ClientUpdate update, [FromServices] Validation validation)
        {
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			validation.AddValidator(new IsUserAdmin(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(403,StatusMessages.AccessDenied);
			}
			StatusMessages status = _service.Update(update, _dbContext);
			if(StatusMessages.UpdateClient == status)
			{
				_hubContext.Clients.Group("client").SendAsync("AdminNotification",JsonConvert.SerializeObject(new {status="update", type="client",data=update}));
			}
			return StatusCode(status, status);
		}

		[Authorize(Roles = "Administrator")]
        [HttpDelete("api/[controller]/{itemid:guid}/{verificateionCode:int}")]
        public async Task<IActionResult> Delete(Guid itemid, int verificateionCode, [FromServices] TeamService _teamService, [FromServices] CertificateService _certificateService, [FromServices] IConfiguration _configuration, [FromServices] EmailNotificationService _emailNotificationService, [FromServices] Validation validation)
        {
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
            validation.AddValidator(new IsUserAdmin(Request.Headers.Authorization, _userManager));
			validation.AddValidator(new DeleteCodeValidator(_dbContext, _configuration, new DeleteAdminItem() { id = itemid, verificationCode = verificateionCode }));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(StatusMessages.AccessDenied, (string)StatusMessages.AccessDenied);
			}
			DeleteVerification deleteVerification = _dbContext.deleteVerifications.FirstOrDefault(dv => dv.itemId == itemid && dv.isClicked);
			if (deleteVerification == null)
			{
				return StatusCode(StatusMessages.InvalidVerificationCode, (string)StatusMessages.InvalidVerificationCode);
			}

			_dbContext.deleteVerifications.Remove(deleteVerification);
			_dbContext.SaveChanges();
			StatusMessages status = _service.Delete(itemid, _teamService, _certificateService, _dbContext, _configuration);
			if(StatusMessages.DeleteClient == status)
			{
				_hubContext.Clients.Group("client").SendAsync("AdminNotification", JsonConvert.SerializeObject(new {status="delete",type="client",data=itemid}));
			}
			_logger.LogError(status);
			return StatusCode(status, status);
		}
    }
}
