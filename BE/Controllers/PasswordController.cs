using Microsoft.AspNetCore.Mvc;
using DTOModel.PasswordDTO;
using Services;
using DataAccessLayerDB;
using Microsoft.AspNetCore.Authorization;
using EncryptionLayer;
using DataMapper;
using AuthenticationLayer;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;



namespace be.Controllers
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
        private readonly JwtTokenManager _jwtTokenManager;
        public PasswordController(PSDBContext dbContext, PasswordService passwordService, SymmetricEncryption symmetricEncryption, IConfiguration configuration, PasswordDataMapper passwordDataMapper, JwtTokenManager jwtTokenManager)
        {
            _dbContext = dbContext;
            _service = passwordService;
            _symmetricEncryption = symmetricEncryption;
            _configuration = configuration;
            _passwordDataMapper = passwordDataMapper;
            _jwtTokenManager = jwtTokenManager;
        }

        [HttpPost("api/[controller]/[action]")]
        public PasswordDTO GetCertificatePasswordById(PasswordString certificatePasswordPostRequest)
        {
            return _service.GetCertificatePasswordById(_dbContext, _symmetricEncryption, _configuration, _passwordDataMapper, certificatePasswordPostRequest, _jwtTokenManager, Request.Headers.Authorization);
        }

        [HttpPost("api/[controller]/[action]")]
        public PasswordDTO GetCredentialPasswordById(PasswordString credentialPasswordPostRequest)
        {
            return _service.GetCredentialPasswordById(_dbContext, _symmetricEncryption, _configuration, _passwordDataMapper, credentialPasswordPostRequest, _jwtTokenManager,Request.Headers.Authorization);
		}

        [HttpPost("api/[controller]/[action]")]
        public PasswordDTO GetPersonalPasswordById(PasswordString personalPassword)
        {

            return _service.GetPersonalPasswordById(_dbContext, _symmetricEncryption, _configuration, _passwordDataMapper, personalPassword, _jwtTokenManager, Request.Headers.Authorization);

		}
    
    }
}
