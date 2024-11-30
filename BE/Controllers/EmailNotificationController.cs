using DataAccessLayerDB;
using DTO.User;
using Microsoft.AspNetCore.Mvc;
using DomainModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using DTO;
using AAAService.Core;
using EmailService;
using AAAService.Core;
using AAAService.Validators;
using LogginMessages;


namespace BE.Controllers
{

	public class EmailNotificationController : Controller
	{
		private readonly PSDBContext _dbContext;
		private readonly UserManager<DomainModel.User> _userManager;
		private readonly JwtManager _jwtManager;
		private readonly IConfiguration _configuration;

		public EmailNotificationController(PSDBContext dbContext, UserManager<DomainModel.User> userManager, JwtManager jwtManager, IConfiguration configuration)
		{
			_dbContext = dbContext;
			_userManager = userManager;
			_jwtManager = jwtManager;
			_configuration = configuration;
		}

		[HttpGet]
		[ResponseCache(NoStore = true)]
		[Route("user/SetNewPassword/{id:guid}")]
		public IActionResult SetNewPassword(Guid id)
		{
			SetNewPassword setNewPassword = new SetNewPassword() { requestid = id };
			EmailNotification changeRequest = _dbContext.EmailNotifiers.Find(id);
			if (changeRequest == null)
			{
				return StatusCode(StatusMessages.ResourceNotFound, StatusMessages.ResourceNotFound);
			}
			if (changeRequest.isClicked)
			{
				return StatusCode(StatusMessages.ResourceNotFound, StatusMessages.ResourceNotFound);
			}

			if(!Int32.TryParse(_configuration.GetSection("PasswordSetResetExpirationTime").Value, out int passwordExpirationTime))
			{
				return StatusCode(StatusMessages.UnableToService,StatusMessages.UnableToService);
			}

			if ((DateTime.Now - changeRequest.createdon).Minutes > passwordExpirationTime)
			{
				return StatusCode(StatusMessages.ResourceNotFound, StatusMessages.ResourceNotFound);
			}

			return View(setNewPassword);

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("user/SetNewPassword/{id:guid}")]
		public async Task<IActionResult> SetNewPassword(Guid id,SetNewPassword setNewPassword )
		{
			EmailNotification changeRequest = _dbContext.EmailNotifiers.Include(en => en.user).FirstOrDefault(en => en.Id == id);
			if (changeRequest == null)
			{
				return StatusCode(StatusMessages.ResourceNotFound, StatusMessages.ResourceNotFound);
			}

			if (changeRequest.isClicked)
			{
				return StatusCode(StatusMessages.ResourceNotFound, StatusMessages.ResourceNotFound);
			}

			if (!Int32.TryParse(_configuration.GetSection("PasswordSetResetExpirationTime").Value, out int ExpirationTime))
			{
				return StatusCode(StatusMessages.UnableToService, StatusMessages.UnableToService);
			}

			if ((DateTime.Now - changeRequest.createdon).Minutes > ExpirationTime && (DateTime.Now - changeRequest.createdon).Hours == 0 && (DateTime.Now - changeRequest.createdon).Days == 0)
			{
				return StatusCode(StatusMessages.ResourceNotFound, StatusMessages.ResourceNotFound);
			}
			if (setNewPassword.password != setNewPassword.confirmpassword)
			{
				ViewData["ErrorMessage"] = "Confirmation password is not same";
				return View(setNewPassword);
			}
			
			if(changeRequest.type == TypeEnum.SetNewPassword)
			{
				IdentityResult result = await _userManager.AddPasswordAsync(changeRequest.user, setNewPassword.password);
				if (result.Succeeded)
				{
					_dbContext.EmailNotifiers.Remove(changeRequest);
					_dbContext.SaveChanges();
					return Redirect("https://localhost:4200/");
				}
				else
				{
					return RedirectToAction("SetNewPassword");
				}
			}
			else if(changeRequest.type == TypeEnum.ResetPassword)
			{
				IdentityResult result = await _userManager.ResetPasswordAsync(changeRequest.user, changeRequest.validationCode, setNewPassword.password);
				if(result.Succeeded)
				{
					_dbContext.EmailNotifiers.Remove(changeRequest);
					_dbContext.SaveChanges();
					return Redirect("https://localhost:4200/");
				}
			}
			return StatusCode(StatusMessages.ResourceNotFound, StatusMessages.ResourceNotFound);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost("deleterequest")]
		public async Task<IActionResult> DeleteVerificationRequest([FromBody] DeleteAdminRequest item, [FromServices] MailJetMailer _mailjetMailer, [FromServices] IConfiguration _configuration, [FromServices] Validation validation)
		{
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			validation.AddValidator(new IsUserAdmin(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(404,StatusMessages.ResourceNotFound);
			}
			(_, Guid userid) = _jwtManager.GetUserID(Request.Headers.Authorization);
			DomainModel.User user = _dbContext.Users.Find(userid.ToString());
			if(user == null)
			{
				return StatusCode(404,StatusMessages.ResourceNotFound);
			}

			bool flag = Enum.TryParse(typeof(ItemType), item.type, out object type);
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
				return StatusCode(404,StatusMessages.ResourceNotFound);
			}

		}



	}
}
