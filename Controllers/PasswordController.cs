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
        public PasswordDTO getPasswordByID(Guid passwordid)
        {
            PasswordService passwordService = new PasswordService(_dbContext);
            return passwordService.GetPasswordById(passwordid);
        }
    }
}
