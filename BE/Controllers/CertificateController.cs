using Microsoft.AspNetCore.Mvc;
using Services;
using DTOModel;
using DataAccessLayerDB;
using DataMapper;
using AuthenticationLayer;
using DomainModel;
using EncryptionLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using DTOModel.TeamDTO;
using Org.BouncyCastle.Tls;
using Org.BouncyCastle.Ocsp;


namespace be.Controllers
{
    [ApiController]
    [ResponseCache(NoStore = true)]
    [Authorize(Roles = "User")]
    [Route("api/[controller]/[action]")]
    public class CertificateController : ControllerBase
    {
        private readonly PSDBContext _dbContext;
        private readonly CertificateService _service;
        private readonly CertificateDataMapper _dataMapper;
        private readonly JwtTokenManager _jwtTokenManager;
        private readonly IConfiguration _configuration;
        private readonly UserAuthorization _userAuthorization;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CertificateController(PSDBContext dbContext, CertificateService service, CertificateDataMapper dataMapper, JwtTokenManager jwtTokenManager, IConfiguration configuration, UserAuthorization userAuthorization, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _service = service;
            _dataMapper = dataMapper;
            _jwtTokenManager = jwtTokenManager;
            _configuration = configuration;
            _userAuthorization = userAuthorization;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet("{clientid:guid}")]
        public IEnumerable<CertificateDTO> GetCertificateByClientID(Guid clientid)
        {
            return _service.GetCertificatesByClintId(clientid, _dataMapper, _jwtTokenManager, Request.Headers.Authorization, _dbContext);
        }


        [HttpPost]
        public async Task<IActionResult> UploadCertificate([FromForm] UploadCertificatecs up, [FromServices] SymmetricEncryption _symmetricEncryption, [FromServices] ILogger<UserAuthorization> _logger)
        {

            if (_jwtTokenManager.GetUserID(Request.Headers.Authorization, out Guid _userid))
            {
				if (ModelState.IsValid)
				{
					return StatusCode(200, _service.UploadCertificate(_configuration, _dbContext, _userAuthorization, _symmetricEncryption, up, _userid, _logger));
				}
                return StatusCode(409);
            }
            return StatusCode(401);
        }

        [HttpGet("{teamid:guid}/{certificateid:guid}")]
        public FileContentResult DownloadCertificate(Guid teamid, Guid certificateid)
        {
            RequestDownloadCertificate downloadCertificate = new RequestDownloadCertificate() { certificateId = certificateid, teamId= teamid };
			ResponseDownloadCertificate output = _service.DownloadCertificate(_dbContext,_configuration,_jwtTokenManager,Request.Headers.Authorization,downloadCertificate);
            Response.Headers.Add("Content-Disposition", $"{output.filename}");
            return output.FileContent;
		}

		[HttpGet("{teamid:guid}/{certificateid:guid}")]
		public FileContentResult DownloadKey(Guid teamid, Guid certificateid)
		{
			RequestDownloadCertificate downloadCertificate = new RequestDownloadCertificate() { certificateId = certificateid, teamId = teamid };
			ResponseDownloadCertificate output = _service.DownloadCertificateKey(_dbContext, _configuration, _jwtTokenManager, Request.Headers.Authorization, downloadCertificate);
			Response.Headers.Add("Content-Disposition", $"{output.filename}");
			return output.FileContent;
		}

		[HttpDelete]
        public async Task<IActionResult> Delete(DeleteItem certItem, [FromServices] ILogger<UserAuthorization> _logger)
        {
            if(await _userAuthorization.IsValidLoggedUser(_userManager,_roleManager, _jwtTokenManager,Request.Headers.Authorization,_logger))
            {
                
				return StatusCode(200,_service.Delete(certItem,_dbContext,_configuration,_jwtTokenManager,Request.Headers.Authorization));
			}
			return StatusCode(404);
		}
    }
}
