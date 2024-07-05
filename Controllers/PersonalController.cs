using DTOModel.CredentialDTO;
using DTOModel.PersonalDTO;
using Microsoft.AspNetCore.Mvc;
using Services;
using DTOModel;
using DataAccessLayerDB;
using DomainModel.DB;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace be.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PersonalController : ControllerBase
    {
        private readonly PSDBContext _dbContext;

        public PersonalController(PSDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("{userid}/{folderid}")]
        public IEnumerable<CredentialDTO> GetCredentialsByFoderId(Guid userid,Guid folderid)
        {
            CredentialService credentialService = new CredentialService(_dbContext);
            return credentialService.GetCredentialsByUserId(userid,folderid);
        }

        [HttpGet("{userid}")]
        public IEnumerable<PersonalFolderDTO> GetCredentialsFoldersByUserID(Guid userid)
        {
            PersonalService personalService = new PersonalService(_dbContext);
            return personalService.GetPersonalFoldersByUserid(userid);
        }
        
        [HttpGet("{userid}/{folderid}")]
        public IEnumerable<PersonalFolderDTO> GetCredentialsFolderByFoderID(Guid userid, Guid folderid)
        {
            PersonalService personalService = new PersonalService(_dbContext);
            return personalService.GetCredentialsFolderByFoderID(userid, folderid);
        }
        
        [HttpPost("{userid}")]
        public string AddFolderByUserId(Guid userId, PostPersonalFolderDTO FolderName)
        {
            PersonalService personalService = new PersonalService(_dbContext);
            return JsonSerializer.Serialize(personalService.AddPersonalFolder(FolderName.name,userId));
        }

        [HttpPost("{userid}")]
        public string AddCredentialByUserId(Guid userid, PostPersonalCredentialDTO postPersonalCredential)
        {
            PersonalService personalService = new PersonalService(_dbContext);
            return JsonSerializer.Serialize(personalService.AddPersonalCredential(postPersonalCredential, userid));
        }
    }
}
