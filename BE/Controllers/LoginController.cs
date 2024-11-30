using DTO.Login;
using DomainModel;
using AAAService.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using DataAccessLayerDB;
using AAAService.Validators;
using LogginMessages;
namespace BE.Controllers
{

	[ApiController]
	[ResponseCache(NoStore = true)]
	public class LoginController : ControllerBase
	{
		private readonly UserManager<User> _userManager;
		private readonly IConfiguration _configuration;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly PSDBContext _dbContext;
		private readonly ILogger<object> _logger;
		public LoginController(UserManager<User> userManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager, PSDBContext dbContext, ILogger<object> logger)
		{
			_userManager = userManager;
			_configuration = configuration;
			_roleManager = roleManager;
			_dbContext = dbContext;
			_logger = logger;
		}

		[HttpPost("[controller]")]
		public async Task<IActionResult> SignIn(Login loginUser, [FromServices] Validation validation, [FromServices] JwtManager _jwtManager)
		{
			validation.AddValidator(new PasswordValidator(loginUser.username, loginUser.password, _userManager, _dbContext));
			
			if(!(await validation.ProcessAsync()))
			{
				return StatusCode(StatusMessages.UnauthorizedAccess, StatusMessages.UnauthorizedAccess);
			}
			User logged = null;
			try
			{
				logged = _dbContext.Users.FirstOrDefault(u => u.UserName.ToLower() == loginUser.username.ToLower());
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return StatusCode(StatusMessages.UnauthorizedAccess, StatusMessages.UnauthorizedAccess);
			}
			try
			{
				return StatusCode(StatusMessages.Ok, await _jwtManager.CreateToken(_userManager, logged, _configuration, _roleManager, _logger));
			}
			catch
			{
				_logger.LogError($"{DateTime.Now} - Jwt Token can not be created");
				return StatusCode(StatusMessages.UnauthorizedAccess, StatusMessages.UnauthorizedAccess);
			}
			
		}
	}
}
