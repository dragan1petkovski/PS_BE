using Microsoft.AspNetCore.Mvc;
using Services;
using DTOModel;
using DataAccessLayerDB;


namespace be.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CertificateController : ControllerBase
    {
        private readonly PSDBContext _dbContext;
        public CertificateController(PSDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet("{userid}/{clientid}")]
        public IEnumerable<CertificateDTO> GetCertificateByClientID(Guid userid, Guid clientid)
        {
            CertificateService certificateService = new CertificateService(_dbContext);
            
            return certificateService.GetCertificatesByUserIdClintId(userid, clientid);
        }
    }
}
