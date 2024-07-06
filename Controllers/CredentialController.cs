using DataAccessLayerDB;
using DTOModel.CredentialDTO;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Text.Json;

namespace be.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CredentialController : ControllerBase
    {
        private readonly PSDBContext _dbContext;
        public CredentialController(PSDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet("{userid}/{clientid}")]
        public IEnumerable<CredentialDTO> GetCredentialByClientID( Guid userid,Guid clientid)
        {
            CredentialService credentialService = new CredentialService(_dbContext);
            
            return credentialService.GetCredentialsByUserIdClintId(clientid, userid);
        }

        [HttpGet("{userid}")]
        public IEnumerable<CredentialDTO> GetCredentialByUserID(Guid userid)
        {
            return new List<CredentialDTO>();
        }

        [HttpPost]
        public string SetCredential(PostCredentialDTO credential)
        {
            CredentialService credentialService = new CredentialService(_dbContext);

            return JsonSerializer.Serialize(credentialService.AddCredential(credential));
            //return "nesto";
        }

        [HttpDelete("{userid}/{teamid}/{credentialid}")]
        public string DeleteCredential(Guid userid,Guid teamid, Guid credentialid)
        {
            CredentialService credentialService = new CredentialService(_dbContext);

            return JsonSerializer.Serialize(credentialService.DeleteCredential(userid,teamid,credentialid));
        }

        [HttpPost]
        public string GiveCredential(PostGiveCredentialDTO giveCredential)
        {
            CredentialService credentialService = new CredentialService( _dbContext);

            return JsonSerializer.Serialize(credentialService.GiveCredential(giveCredential));
        }
    }
}
