using Microsoft.AspNetCore.Mvc;
using Services;
using DTOModel;


namespace be.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CertificateController : ControllerBase
    {
        private readonly IConfiguration _config;
        public CertificateController(IConfiguration config)
        {
            _config = config;
        }
        [HttpGet("{userid}/{clientid}")]
        public IEnumerable<CertificateDTO> GetCertificateByClientID(Guid userid, Guid clientid)
        {
            return new List<CertificateDTO>();
        }
    }
}
