using DTO.Credential;
using DTO.Personal;
using Microsoft.AspNetCore.Mvc;
using AppServices;
using DataAccessLayerDB;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using DTO;
using EncryptionLayer;
using Microsoft.AspNetCore.Identity;
using DomainModel;
using AAAService.Core;
using AAAService.Validators;
using DTO.Client;
using LogginMessages;


namespace BE.Controllers
{
    [ApiController]
	[Authorize(Roles = "User")]
    public class PersonalFolderController : ControllerBase
    {
        private readonly PSDBContext _dbContext;
        private readonly JwtManager _jwtManager;
        private readonly PersonalService _service;
        private readonly UserManager<DomainModel.User> _userManager;

        public PersonalFolderController(PSDBContext dbContext, JwtManager jwtManager, [FromServices] PersonalService personalService, UserManager<DomainModel.User> userManager)
        {
            _dbContext = dbContext;
            _jwtManager = jwtManager;
			_service = personalService;
            _userManager = userManager;
        }

        [HttpGet("api/[controller]/{pfid:guid?}")]
        public async Task<IActionResult> GetCredentialFoldersByUserID(Guid? pfid,[FromServices] Validation _validation)
        {
			_validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			if (!(await _validation.ProcessAsync()))
			{
				return StatusCode(StatusMessages.AccessDenied, StatusMessages.AccessDenied);
			}
			(_, Guid userid) = _jwtManager.GetUserID(Request.Headers.Authorization);

            if(!pfid.HasValue)
            {
				(StatusMessages statusCode, List<DTO.Personal.PersonalFolder> output) = _service.GetPersonalFolders(userid, _dbContext);
				if (StatusMessages.Ok == statusCode)
				{
					return StatusCode(statusCode, output);
				}
				return StatusCode(statusCode, statusCode);
			}
			else
			{
				(StatusMessages statusCode, DTO.Personal.PersonalFolder output) = _service.GetPersonalFolderById(userid, pfid.HasValue ? pfid.Value : Guid.Empty, _dbContext);
				if (StatusMessages.Ok == statusCode)
				{
					return StatusCode(statusCode, output);
				}
				return StatusCode(statusCode, statusCode);
			}	



		}

		[HttpPut("api/[Controller]/{personalFolderId:guid}")]
		public async Task<IActionResult> Update(Guid personalFolderId, PostPersonalFolder folderName, [FromServices] Validation _validation)
		{
			{
				_validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
				if (!(await _validation.ProcessAsync()))
				{
					return StatusCode(StatusMessages.AccessDenied, StatusMessages.AccessDenied);
				}
				(_, Guid userid) = _jwtManager.GetUserID(Request.Headers.Authorization);
				StatusMessages status = _service.Update(userid, personalFolderId, folderName.name, _dbContext);
				return StatusCode(status, status);
			}
		}

        [HttpPost("api/[controller]")]
        public async Task<IActionResult> AddFolderByUserId(PostPersonalFolder folderName, [FromServices] Validation _validation)
        {
			_validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			if (!(await _validation.ProcessAsync()))
			{
				return StatusCode(StatusMessages.AccessDenied, StatusMessages.AccessDenied);
			}
			(_, Guid userid) = _jwtManager.GetUserID(Request.Headers.Authorization);
            StatusMessages status = _service.AddPersonalFolder(folderName.name, userid, _dbContext);
			return StatusCode(status, status);
        }

        [HttpDelete("api/[controller]/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, [FromServices] UserManager<User> _userManager, [FromServices] RoleManager<IdentityRole> _roleManager, [FromServices] Validation _validation)
        {
			_validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			if (!(await _validation.ProcessAsync()))
			{
				return StatusCode(StatusMessages.AccessDenied, StatusMessages.AccessDenied);
			}
			(_, Guid userid) = _jwtManager.GetUserID(Request.Headers.Authorization);
			StatusMessages status = _service.Delete(id,userid, _dbContext);
			return StatusCode(status, status);
		}	
    }
}
