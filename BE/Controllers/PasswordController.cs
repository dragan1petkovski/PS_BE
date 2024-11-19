using Microsoft.AspNetCore.Mvc;
using DTO.Password;
using AppServices;
using DataAccessLayerDB;
using Microsoft.AspNetCore.Authorization;
using EncryptionLayer;
using DataMapper;
using AAAService.Core;
using AAAService.Validators;
using Microsoft.AspNetCore.Identity;
using DomainModel;
using LogginMessages;



namespace BE.Controllers
{
    [ApiController]
    [Authorize(Roles = "User")]
	[ResponseCache(NoStore = true)]
	public class PasswordController : ControllerBase
    {
        private readonly PSDBContext _dbContext;
        private readonly PasswordService _service;
        private readonly SymmetricEncryption _symmetricEncryption;
        private readonly IConfiguration _configuration;
        private readonly PasswordDataMapper _passwordDataMapper;
        private readonly JwtManager _jwtManager;
        private readonly UserManager<DomainModel.User> _userManager;
        public PasswordController(PSDBContext dbContext, PasswordService passwordService, SymmetricEncryption symmetricEncryption, IConfiguration configuration, PasswordDataMapper passwordDataMapper, JwtManager jwtManager, UserManager<DomainModel.User> userManager)
        {
            _dbContext = dbContext;
            _service = passwordService;
            _symmetricEncryption = symmetricEncryption;
            _configuration = configuration;
            _passwordDataMapper = passwordDataMapper;
            _jwtManager = jwtManager;
            _userManager = userManager;
        }
		
		[HttpGet("api/[controller]/certificate/{id:guid:required}/{parentid:guid}")]
		public async Task<IActionResult> CertificatePassword(Guid id, Guid parentid, [FromServices] Validation validation)
        {
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(StatusMessages.AccessDenied, StatusMessages.AccessDenied);
			}
			(_, Guid userid) = _jwtManager.GetUserID(Request.Headers.Authorization);
			(StatusMessages status, DTO.Password.Password password) = _service.GetCertificatePasswordById(userid,_dbContext, _symmetricEncryption, _configuration, _passwordDataMapper, id, parentid);
			if(StatusMessages.Ok == status)
			{
				return StatusCode(status, password);
			}
			return StatusCode(status, status);
        }

		[HttpGet("api/[controller]/credential/{id:guid:required}/{parentid:guid}")]
		public async Task<IActionResult> CredentialPassword(Guid id, Guid parentid, [FromServices] Validation validation)
        {
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(StatusMessages.AccessDenied, StatusMessages.AccessDenied);
			}
			(_, Guid userid) = _jwtManager.GetUserID(Request.Headers.Authorization);
			(StatusMessages status, DTO.Password.Password password) = _service.GetCredentialPasswordById(userid, _dbContext, _symmetricEncryption, _configuration, _passwordDataMapper,id, parentid);
			if (StatusMessages.Ok == status)
			{
				return StatusCode(status, password);
			}
			return StatusCode(status, status);
		}

		[HttpGet("api/[controller]/personal/{id:guid:required}/{parentid:guid?}")]
		public async Task<IActionResult> PersonalPassword(Guid id, Guid parentid, [FromServices] Validation validation)
        {
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(StatusMessages.AccessDenied, StatusMessages.AccessDenied);
			}
			(_, Guid userid) = _jwtManager.GetUserID(Request.Headers.Authorization);
			(StatusMessages status, DTO.Password.Password password) = _service.GetPersonalPasswordById(userid, _dbContext, _symmetricEncryption, _configuration, _passwordDataMapper, id, parentid);
			if (StatusMessages.Ok == status)
			{
				return StatusCode(status, password);
			}
			return StatusCode(status, status);

		}
    
    }
}
