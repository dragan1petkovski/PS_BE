using Microsoft.AspNetCore.Mvc;
using AppServices;
using DTO.User;
using DataAccessLayerDB;
using DataMapper;
using Microsoft.AspNetCore.Authorization;
using DomainModel;
using EmailService;
using DTO;
using Microsoft.AspNetCore.Identity;
using AAAService.Core;
using DTO.Team;
using AAAService.Validators;
using DTO.Client;
using LogginMessages;
using Microsoft.AspNetCore.SignalR;
using SignalR;
using Newtonsoft.Json;


namespace BE.Controllers
{
	[ApiController]
	[ResponseCache(NoStore = true)]
	public class UserController : ControllerBase
	{
		private readonly PSDBContext _dbContext;
		private readonly UserService _service;
		private readonly UserDataMapper _dataMapper;
		private readonly IConfiguration _configuration;
		private readonly UserManager<DomainModel.User> _userManager;
		private readonly JwtManager _jwtManager;
		private readonly ILogger<UserService> _logger;
		private readonly IHubContext<DataSync> _hubContext;
		private readonly iEmailService _emailService;

		public UserController(PSDBContext dbContext, UserService service, UserDataMapper dataMapper, IConfiguration configuration, UserManager<DomainModel.User> userManager, JwtManager jwtManager, MailJetMailer emailService, ILogger<UserService> logger, IHubContext<DataSync> hubContext)
		{
			_dbContext = dbContext;
			_service = service;
			_dataMapper = dataMapper;
			_configuration = configuration;
			_userManager = userManager;
			_jwtManager = jwtManager;
			_logger = logger;
			_hubContext = hubContext;
			_emailService = emailService;
		}

		[HttpGet("[controller]/{type?}/{id:guid?}")]
		[Authorize(Roles = "Administrator, User")]
		public async Task<IActionResult> Read(string? type, Guid? id, [FromServices] Validation validation)
		{

			if (type == "all")
			{

				validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
				validation.AddValidator(new IsUserAdmin(Request.Headers.Authorization, _userManager));
				if (!(await validation.ProcessAsync()))
				{
					return StatusCode(403, StatusMessages.AccessDenied);
				}
				_logger.LogError($"Query parameter value: {id}");
				(bool _, Guid userid) = _jwtManager.GetUserID(Request.Headers.Authorization);
				if (!id.HasValue)
				{
					return StatusCode(200, (await _dataMapper.ConvertUserListToUserFullDTOList(_service.GetAllUsers(_dbContext),_userManager)).Where(u => u.id != userid).ToList());
				}
				else
				{

					return StatusCode(200, _service.GetUserDetails(id.HasValue ? id.Value : Guid.Empty, _dbContext, _logger));
				}

			}
			else
			{
				validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
				if (!(await validation.ProcessAsync()))
				{
					return StatusCode(403, StatusMessages.AccessDenied);
				}
				return StatusCode(200, _dataMapper.ConvertUserListToUserPartDTOList(await _dataMapper.GetUsersWithRoleAsync(_userManager, "User", _service.GetAllUsers(_dbContext))));
			}
		}

		[HttpGet("[controller]/[action]")]
		[Authorize(Roles = "Administrator, User")]
		public async Task<IActionResult> VerificationCode([FromServices] Validation validation)
		{
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(401, StatusMessages.UnauthorizedAccess);
			}
			(_, Guid userid) = _jwtManager.GetUserID(Request.Headers.Authorization);
			StatusMessages status = await _service.GetVerificationCode(userid, _dbContext, _configuration, _emailService, _logger);
			return StatusCode(status, status);
		}


		[HttpPost("[controller]/{type?}")]
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> Create(string? type, PostUser user, [FromServices] Validation validation)
		{
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			validation.AddValidator(new IsUserAdmin(Request.Headers.Authorization, _userManager));
			StatusMessages status = null;
			DTO.User.User data = null;
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(StatusMessages.AccessDenied, StatusMessages.AccessDenied);
			}
			if(type == "admin")
			{
				(status, data) = await _service.AddUser("admin", user, _dbContext, _configuration, _emailService, _logger);
			}
			else
			{
				(status, data) = await _service.AddUser("user",user, _dbContext, _configuration, _emailService, _logger);
			}
			
			if(status == StatusMessages.AddNewUser)
			{
				_hubContext.Clients.Group("user").SendAsync("AdminNotification", JsonConvert.SerializeObject(new { status = "new", type = "user", data = data }));
			}
			return StatusCode(status, status);
		}


		[HttpPost("[controller]/[action]")]
		[Authorize(Roles = "Administrator, User")]
		public async Task<IActionResult> ChangePassword(ChangePassword changePassword, [FromServices] Validation validation)
		{
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			validation.AddValidator(new IsUserAdmin(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(401, StatusMessages.UnauthorizedAccess);
			}
			(_, Guid userid) = _jwtManager.GetUserID(Request.Headers.Authorization);
			StatusMessages status = await _service.ChangePassword(userid, _dbContext, _userManager, changePassword, _logger);
			return StatusCode(status, status);
		}

		[HttpPut("[controller]")]
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> Update(PostUpdateUser updateUserDTO, [FromServices] Validation validation)
		{
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			validation.AddValidator(new IsUserAdmin(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(401, StatusMessages.UnauthorizedAccess);
			}
			(StatusMessages status, DTO.User.User data) = _service.Update(updateUserDTO, _dbContext, _logger);
			if(status == StatusMessages.UpdateUser)
			{
				_hubContext.Clients.Group("user").SendAsync("AdminNotification", JsonConvert.SerializeObject(new { status = "update", type = "user", data = data }));
			}
			return StatusCode(status, status);
		}

		[HttpPut("[controller]/[action]/{userid:guid:required}")]
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> ResetPassword(Guid userid, [FromServices] Validation validation)
		{
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			validation.AddValidator(new IsUserAdmin(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(401, StatusMessages.UnauthorizedAccess);
			}
			StatusMessages status = await _service.ResetPassword(userid, _dbContext, _userManager, _emailService, _configuration, _logger);
			return StatusCode(status, status);

		}

        [HttpDelete("[controller]/{itemid:guid}/{verificateionCode:int}")]
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> Delete(Guid itemid, int verificateionCode,[FromServices] EmailNotificationService _emailNotificationService, [FromServices] Validation validation)
        {
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			validation.AddValidator(new IsUserAdmin(Request.Headers.Authorization, _userManager));
			validation.AddValidator(new DeleteCodeValidator(_dbContext, _configuration, new DeleteAdminItem() { id =itemid, verificationCode= verificateionCode}));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(StatusMessages.AccessDenied,(string)StatusMessages.AccessDenied);
			}
			DeleteVerification deleteVerification = _dbContext.deleteVerifications.FirstOrDefault(dv => dv.itemId == itemid && dv.isClicked);
			if (deleteVerification == null)
			{
				return StatusCode(StatusMessages.InvalidVerificationCode, (string)StatusMessages.InvalidVerificationCode);
			}

			_dbContext.deleteVerifications.Remove(deleteVerification);
			_dbContext.SaveChanges();
			StatusMessages status = _service.Delete(itemid, _dbContext, _logger);
			if(StatusMessages.DeleteUser == status)
			{
				_hubContext.Clients.Group("user").SendAsync("AdminNotification", JsonConvert.SerializeObject(new { status = "delete", type = "user", data = itemid }));
			}
			return StatusCode(status, (string)status);
        }
    }
}
