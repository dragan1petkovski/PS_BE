using Microsoft.AspNetCore.Mvc;
using DTOModel.PasswordDTO;
using Services;
using DataAccessLayerDB;
namespace be.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PasswordController : ControllerBase
    {
        private readonly PSDBContext _dbContext;
        public PasswordController(PSDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet("{passwordid}")]
        public PasswordDTO GetCredentialPasswordById(Guid passwordid)
        {
            PasswordService passwordService = new PasswordService(_dbContext);
            return passwordService.GetCredentialPasswordById(passwordid);
        }

        [HttpGet("{passwordid}")]
        public PasswordDTO GetCertificatePasswordById(Guid passwordid)
        {
            PasswordService passwordService = new PasswordService(_dbContext);
            return passwordService.GetCertificatePasswordById(passwordid);
        }
    }
}
