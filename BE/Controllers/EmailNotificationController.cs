using DataAccessLayerDB;
using DTOModel.UserDTO;
using Microsoft.AspNetCore.Mvc;
using DomainModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using DTOModel;
using AuthenticationLayer;
using EmailService;
using Microsoft.Extensions.Configuration;


namespace BE.Controllers
{

	public class EmailNotificationController : Controller
	{
		private readonly PSDBContext _dbContext;
		private readonly UserManager<User> _userManager;

		public EmailNotificationController(PSDBContext dbContext, UserManager<User> userManager)
		{
			_dbContext = dbContext;
			_userManager = userManager;
		}

		[HttpGet]
		[ResponseCache(NoStore = true)]

		[Route("{Controller}/{Action}/{id:guid}")]
		public IActionResult SetNewPassword(Guid id)
		{
			SetNewPassword setNewPassword = new SetNewPassword() { requestid = id };
			EmailNotification changeRequest = _dbContext.EmailNotifiers.Find(id);
			if (changeRequest != null)
			{
				if (changeRequest.isClicked)
				{
					return StatusCode(404);
				}
				else
				{
					if ((DateTime.Now - changeRequest.createdon).Minutes < 5)
					{
						return View(setNewPassword);
					}
					else
					{
						return StatusCode(404);
					}
				}
			}
			else
			{
				return StatusCode(404);
			}

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("{Controller}/{Action}/{id:guid}")]
		public async Task<IActionResult> SetNewPassword(SetNewPassword setNewPassword, Guid id)
		{
			EmailNotification changeRequest = _dbContext.EmailNotifiers.Include(en => en.user).FirstOrDefault(en => en.Id == id);
			if (changeRequest != null)
			{
				if (changeRequest.isClicked)
				{
					return StatusCode(404);
				}
				else
				{
					if ((DateTime.Now - changeRequest.createdon).Minutes < 5 && (DateTime.Now - changeRequest.createdon).Hours == 0 && (DateTime.Now - changeRequest.createdon).Days == 0)
					{
						if (setNewPassword.password == setNewPassword.confirmpassword)
						{
							IdentityResult result = await _userManager.AddPasswordAsync(changeRequest.user, setNewPassword.password);
							if (result.Succeeded)
							{
								_dbContext.EmailNotifiers.Remove(changeRequest);
								_dbContext.SaveChanges();
								return Redirect("https://localhost:4200/");
							}
							return StatusCode(404);
						}
						return View(setNewPassword);

					}
					else
					{
						return StatusCode(404);
					}
				}
			}
			else
			{
				return StatusCode(404);
			}
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost("api/[Controller]/[Action]")]
		public async Task<IActionResult> DeleteVerification([FromBody] DeleteAdminRequest item, [FromServices] JwtTokenManager _jwtTokenManager, [FromServices] UserAuthorization _userAuthorization, [FromServices] MailJetMailer _mailjetMailer, [FromServices] IConfiguration _configuration, [FromServices] ILogger<UserAuthorization> _logger)
		{
			bool flag = Enum.TryParse(typeof(ItemType), item.type, out object type);
			if (await _userAuthorization.IsLoggedUserAdmin(_userManager, _jwtTokenManager, Request.Headers.Authorization, _logger) && flag)
			{
				if (_jwtTokenManager.GetUserID(Request.Headers.Authorization, out Guid _userid))
				{
					User user = _dbContext.Users.Find(_userid.ToString());
					if (user != null)
					{
						Random random = new Random();
						DomainModel.DeleteVerification deleteItem = new DeleteVerification();
						deleteItem.id = Guid.NewGuid();
						deleteItem.requestor = user;
						deleteItem.type = (ItemType)type;
						deleteItem.itemId = item.id;
						deleteItem.isClicked = false;

						int code = random.Next(10000000, 100000000);
						deleteItem.verificationCode = code;
						deleteItem.createdate = DateTime.Now;

						if (await _mailjetMailer.SendMailMessage(_configuration, user.NormalizedEmail, _mailjetMailer.GetVerificationCode(user.NormalizedUserName, code), _mailjetMailer.Subject))
						{
							_dbContext.deleteVerifications.Add(deleteItem);
							_dbContext.SaveChanges();
							return StatusCode(200);
						}
						else
						{
							return StatusCode(404);
						}
					}
					else
					{
						return StatusCode(404);
					}
					
				}
				else
				{
					return StatusCode(404);
				}

			}
			else
			{
				return StatusCode(404);
			}

		}



	}
}
