using Microsoft.AspNetCore.Mvc;
using AppServices;
using DataAccessLayerDB;
using DataMapper;
using EncryptionLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using DTO.Certificate;
using AAAService.Core;
using AAAService.Validators;
using DTO;
using LogginMessages;
using Microsoft.AspNetCore.SignalR;
using SignalR;
using Newtonsoft.Json;


namespace BE.Controllers
{
    [ApiController]
    
    [Authorize(Roles = "User")]
    public class CertificateController : ControllerBase
    {
        private readonly PSDBContext _dbContext;
        private readonly CertificateService _service;
        private readonly CertificateDataMapper _dataMapper;
        private readonly JwtManager _jwtManager;
        private readonly IConfiguration _configuration;
        private readonly UserManager<DomainModel.User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<CertificateService> _logger;
		private readonly IHubContext<DataSync> _hubContext;
        public CertificateController(PSDBContext dbContext, CertificateService service, CertificateDataMapper dataMapper, JwtManager jwtManager, IConfiguration configuration, UserManager<DomainModel.User> userManager, RoleManager<IdentityRole> roleManager, ILogger<CertificateService> logger, IHubContext<DataSync> hubContext)
        {
            _dbContext = dbContext;
            _service = service;
            _dataMapper = dataMapper;
            _jwtManager = jwtManager;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
			_hubContext = hubContext;
        }
        [HttpGet("[controller]/{clientid:guid}")]
        public async Task<IActionResult> GetCertificateByClientID(Guid clientid, [FromServices] Validation validation)
        {
            validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
            if(!(await validation.ProcessAsync()))
            {
                return StatusCode(403, new List<Certificate>());
            }
            (_, Guid userid) = _jwtManager.GetUserID(Request.Headers.Authorization);
            return StatusCode(200,_service.GetCertificatesByClintId(clientid,userid,_dataMapper, _dbContext, _logger));
        }


        [HttpGet("[controller]/download/{teamid:guid}/{certificateid:guid}")]
        public async Task<FileContentResult> DownloadCertificate(Guid teamid, Guid certificateid, [FromServices] Validation validation)
        {
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return null;
			}
			(_, Guid userid) = _jwtManager.GetUserID(Request.Headers.Authorization);

			RequestDownloadCertificate downloadCertificate = new RequestDownloadCertificate() { certificateId = certificateid, teamId= teamid };
			ResponseDownloadCertificate output = _service.DownloadCertificate(_dbContext,_configuration,userid,downloadCertificate);
            Response.Headers.Add("Content-Disposition", $"{output.filename}");
            return output.FileContent;
		}

		[HttpGet("[controller]/key/download/{teamid:guid}/{certificateid:guid}")]
		public async Task<FileContentResult> DownloadKey(Guid teamid, Guid certificateid, [FromServices] Validation validation)
		{
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return null;
			}
			(_, Guid userid) = _jwtManager.GetUserID(Request.Headers.Authorization);
			RequestDownloadCertificate downloadCertificate = new RequestDownloadCertificate() { certificateId = certificateid, teamId = teamid };
			ResponseDownloadCertificate output = _service.DownloadCertificateKey(_dbContext, _configuration, userid, downloadCertificate);
			Response.Headers.Add("Content-Disposition", $"{output.filename}");
			return output.FileContent;
		}


		[HttpPost("[controller]")]
		public async Task<IActionResult> UploadCertificate([FromForm] UploadCertificatecs up, [FromServices] SymmetricEncryption _symmetricEncryption, [FromServices] Validation validation)
		{

			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(403);
			}
			(_, Guid userid) = _jwtManager.GetUserID(Request.Headers.Authorization);
			(StatusMessages status, List<Certificate> data) = _service.UploadCertificate(_configuration, _dbContext, _symmetricEncryption, up, userid, _logger);
			
			if(StatusMessages.AddNewCertificate == status)
			{
				foreach(Certificate i in data)
				{
					_hubContext.Clients.Group(JsonConvert.SerializeObject(new { id = i.teamid, type = "certificate" }))
					.SendAsync("Notification", JsonConvert.SerializeObject(new { status = "new", type = "certificate", data = i }));
				}
			}

			return StatusCode(status, status);

		}

		[HttpDelete("[controller]/{id:guid}/{teamid:guid}")]
		public async Task<IActionResult> Delete(Guid id, Guid teamid, [FromServices] Validation validation)
        {

			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(404); ;
			}
			(_, Guid userid) = _jwtManager.GetUserID(Request.Headers.Authorization);
			StatusMessages status = _service.Delete(id, teamid, _dbContext, _configuration, userid);
			if(status == StatusMessages.DeleteCertificate)
			{
				_hubContext.Clients.Group(JsonConvert.SerializeObject(new { id = teamid, type = "certificate" }))
				.SendAsync("Notification", JsonConvert.SerializeObject(new { status = "delete", type = "certificate", data = id }));
			}
			return StatusCode(status, status);
			
		}
    }
}
