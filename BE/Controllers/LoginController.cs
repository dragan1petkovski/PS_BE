using DTOModel.LoginDTO;
using DomainModel;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using DataAccessLayerDB;
using EncryptionLayer;
using System.Security.Claims;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.CookiePolicy;
using AuthenticationService;
namespace BE.Controllers
{

	[ApiController]
	[Route("api/[controller]/[Action]")]
	[ResponseCache(NoStore = true)]
	public class LoginController : ControllerBase
	{
		private readonly UserManager<User> _userManager;
		private readonly IConfiguration _configuration;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly PSDBContext _dbContext;
		public LoginController(UserManager<User> userManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager, PSDBContext dbContext)
		{
			_userManager = userManager;
			_configuration = configuration;
			_roleManager = roleManager;
			_dbContext = dbContext;
		}

		[HttpPost]
		public async Task<IActionResult> SignIn(LoginDTO loginUser, [FromServices] AuthenticationService.AuthenticationManager authenticationManager, [FromServices] ILogger<AuthenticationManager> _logger)
		{
			//There is information leak in the jwt token when there is connection issue to the database
			User _user = new User();
			try
			{
				_user = await authenticationManager.ValidateUser(loginUser, _userManager, _logger);
			}
			catch (Exception ex)
			{

			}
			if(_user != null )
			{
				string jwt = "";
				try
				{
					jwt = await authenticationManager.CreateToken(_userManager, _user, _configuration, _roleManager,_logger);
					
				}
				catch
				{
					_logger.LogError($"{DateTime.Now} - Jwt Token can not be created");
					return StatusCode(401, ("Error", "Problem createding JWT token"));
				}
				return StatusCode(200, jwt);


			}
			else
			{

				return StatusCode(401, ("Error", "Problem createding JWT token"));
			}
			
		}
	}
}
