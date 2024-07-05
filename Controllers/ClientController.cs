using Microsoft.AspNetCore.Mvc;
using Services;
using DataAccessLayerDB;
using DTOModel.ClientDTO;
using DomainModel.DB;
using System.Text.Json;

namespace be.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ClientController : ControllerBase
    {
        private readonly PSDBContext _dbContext;
        public ClientController(PSDBContext configuration)
        {
            _dbContext = configuration;
        }
        
        [HttpGet("{type}/{userid}")]
        public IEnumerable<ClientDTO> GetClientsByTypeUserId(string type,Guid userid)
        {
            ClientService clientService = new ClientService(_dbContext);
            return clientService.GetAllClientsByUserId(type, userid);
        }

        [HttpGet]
        public IEnumerable<ClientDTO> GetAllFullClients()
        {
            ClientService clientService= new ClientService(_dbContext);
            return clientService.ConvertClientDBDMListToClientDTOList(clientService.GetAllClients());
        }

        [HttpGet]
        public IEnumerable<ClientDTO> GetAllPartClients()
        {
            ClientService clientService = new ClientService(_dbContext);
            return clientService.ConvertClientDBDMListToClientPartDTOList(clientService.GetAllClients());
        }

        [HttpPost]
        public string AddClient(PostClient postClient)
        {
            ClientService clientService = new ClientService(_dbContext);
            return JsonSerializer.Serialize(clientService.AddNewClient(postClient));

        }
    }
}
