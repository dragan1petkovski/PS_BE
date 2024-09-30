using Microsoft.AspNetCore.Mvc;
using Services;
using DTOModel.UserDTO;
using DataAccessLayerDB;
using System.Text.Json;
using DataMapper;
using Microsoft.AspNetCore.Authorization;
using DomainModel;
using EmailService;
using DTOModel;
using Microsoft.AspNetCore.Identity;
using AuthenticationLayer;
using DTOModel.TeamDTO;


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
        private readonly UserManager<User> _userManager;
        private readonly JwtTokenManager _jwtTokenManager;
        private readonly UserAuthorization _userAuthorization;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(PSDBContext dbContext, UserService service, UserDataMapper dataMapper, IConfiguration configuration, UserManager<User> userManager, JwtTokenManager jwtTokenManager, UserAuthorization userAuthorization, RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _service = service;
            _dataMapper = dataMapper;
            _configuration = configuration;
            _userManager = userManager;
            _jwtTokenManager = jwtTokenManager;
            _userAuthorization = userAuthorization;
            _roleManager = roleManager;
        }

        [HttpGet("api/[controller]/[action]")]
		[Authorize(Roles = "Administrator")]
		public IEnumerable<UserDTO> GetAllFullUsers()
        {
            return _dataMapper.ConvertUserListToUserFullDTOList(_service.GetAllUsers(_dbContext));
             
        }
        

        [HttpGet("api/[controller]/[action]")]
		[Authorize(Roles = "Administrator, User")]
		public async Task<IEnumerable<UserPartDTO>> GetAllPartUsers()
        {
            return _dataMapper.ConvertUserListToUserPartDTOList(await _dataMapper.GetUsersWithRoleAsync(_dbContext,_userManager,"User",_service.GetAllUsers(_dbContext)));
        }


        [HttpPost("api/[controller]/[action]")]
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> Create(PostUserDTO user, [FromServices] MailJetMailer smtpclient, [FromServices] ILogger<UserService> _logger)
        {
            return StatusCode(200,await _service.AddUser(user, _dbContext, _configuration, smtpclient,_logger));
        }


		[HttpPost("api/[controller]/[action]")]
		[Authorize(Roles = "Administrator, User")]
		public async Task<IActionResult> ChangePassword(ChangePasswordDTO changePassword, [FromServices] ILogger<UserService> _logger)
		{
            if(await _service.ChangePassword(_dbContext, _userManager, changePassword, _jwtTokenManager, Request.Headers.Authorization,_logger))
            {
				return StatusCode(200,new SetStatus() { status="OK"});
			}
            return StatusCode(403, new SetStatus() { status = "KO"});
		}


		[HttpGet("api/[controller]/[action]")]
		[Authorize(Roles = "Administrator, User")]
		public async Task<IActionResult> GetVerificationCode([FromServices] MailJetMailer emailService, [FromServices] ILogger<UserService> _logger)
		{
            if(await _service.GetVerificationCode(_dbContext, _configuration, emailService, _jwtTokenManager, Request.Headers.Authorization,_logger))
            {
				return StatusCode(200,new SetStatus() { status = "OK"});
			}
			else
            {
                return StatusCode(404, new SetStatus() { status = "KO"});
            }
		}


        [Authorize(Roles = "Administrator")]
        [HttpGet("api/[controller]/[action]/{id:guid}")]
        public UpdateUserDTO Update(Guid id, [FromServices] JwtTokenManager _jwtTokenManager, [FromServices] ILogger<UserService> _logger)
        {
            return  _service.GetUserDetails(id,_dbContext,_jwtTokenManager,Request.Headers.Authorization, _logger);
        }

        [HttpPost("api/[controller]/[action]")]
        [Authorize(Roles = "Administrator")]
		public async Task<IActionResult> Update(PostUpdateUser updateUserDTO, [FromServices] ILogger<UserAuthorization> _logger, [FromServices] ILogger<UserService> _userLogger)
        {
            if(await _userAuthorization.IsValidLoggedUser(_userManager,_roleManager, _jwtTokenManager,Request.Headers.Authorization, _logger))
            {
                
				return StatusCode(200, _service.Update(updateUserDTO, _dbContext, _userLogger));
			}
            return StatusCode(404);
        }

        [HttpDelete("api/[controller]/[action]")]
        public async Task<IActionResult> Delete([FromBody] DeleteAdminItem deleteitem, [FromServices] EmailNotificationService _emailNotificationService, [FromServices] ILogger<UserAuthorization> _logger, [FromServices] ILogger<UserService> _userLogger)
        {
            if(await _userAuthorization.IsValidLoggedUser(_userManager,_roleManager,_jwtTokenManager,Request.Headers.Authorization, _logger))
            {
                if(_emailNotificationService.TryValidateDeleteCode(_dbContext,deleteitem, out Guid _deleteVerificationId))
                {
                    DeleteVerification deleteVerification =  _dbContext.deleteVerifications.Find(_deleteVerificationId);
                    if(deleteVerification != null)
                    {
                        _dbContext.deleteVerifications.Remove(deleteVerification);
                        _dbContext.SaveChanges();
						return StatusCode(200, _service.Delete(deleteitem.id, _dbContext, _userLogger));
					}
					return StatusCode(404);
				}
				return StatusCode(404);
			}
            return StatusCode(404);
        }
    }
}
