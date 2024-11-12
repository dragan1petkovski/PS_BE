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


namespace be.Controllers
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
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ILogger<UserService> _logger;

		public UserController(PSDBContext dbContext, UserService service, UserDataMapper dataMapper, IConfiguration configuration, UserManager<DomainModel.User> userManager, JwtManager jwtManager, RoleManager<IdentityRole> roleManager, ILogger<UserService> logger)
		{
			_dbContext = dbContext;
			_service = service;
			_dataMapper = dataMapper;
			_configuration = configuration;
			_userManager = userManager;
			_jwtManager = jwtManager;
			_roleManager = roleManager;
			_logger = logger;
		}

		[HttpGet("api/[controller]/{type?}/{id:guid?}")]
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
				if (!id.HasValue)
				{
					return StatusCode(200, _dataMapper.ConvertUserListToUserFullDTOList(_service.GetAllUsers(_dbContext)));
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
				return StatusCode(200, _dataMapper.ConvertUserListToUserPartDTOList(await _dataMapper.GetUsersWithRoleAsync(_dbContext, _userManager, "User", _service.GetAllUsers(_dbContext))));
			}
		}

		[HttpGet("api/[controller]/[action]")]
		[Authorize(Roles = "Administrator, User")]
		public async Task<IActionResult> VerificationCode([FromServices] MailJetMailer emailService, [FromServices] Validation validation)
		{
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(401, StatusMessages.UnauthorizedAccess);
			}
			(_, Guid userid) = _jwtManager.GetUserID(Request.Headers.Authorization);
			StatusMessages status = await _service.GetVerificationCode(userid, _dbContext, _configuration, emailService, _logger);
			return StatusCode(status, status);
		}


		[HttpPost("api/[controller]")]
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> Create(PostUser user, [FromServices] MailJetMailer smtpclient, [FromServices] Validation validation)
		{
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			validation.AddValidator(new IsUserAdmin(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(StatusMessages.AccessDenied, StatusMessages.AccessDenied);
			}
			StatusMessages status = await _service.AddUser(user, _dbContext, _configuration, smtpclient, _logger);
			return StatusCode(status, status);
		}


		[HttpPost("api/[controller]/[action]")]
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

		[HttpPut("api/[controller]")]
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> Update(PostUpdateUser updateUserDTO, [FromServices] Validation validation)
		{
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			validation.AddValidator(new IsUserAdmin(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(401, StatusMessages.UnauthorizedAccess);
			}
			StatusMessages status = _service.Update(updateUserDTO, _dbContext, _logger);
			return StatusCode(status, status);
		}

		[HttpPut("api/[controller]/[action]/{userid:guid:required}")]
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> ResetPassword(Guid userid, [FromServices] Validation validation, [FromServices] MailJetMailer smtpclient)
		{
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			validation.AddValidator(new IsUserAdmin(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(401, StatusMessages.UnauthorizedAccess);
			}
			StatusMessages status = await _service.ResetPassword(userid, _dbContext, _userManager, smtpclient, _configuration, _logger);
			return StatusCode(status, status);

		}

        [HttpDelete("api/[controller]/{itemid:guid}/{verificateionCode:int}")]
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
			return StatusCode(status, (string)status);
        }
    }
}
