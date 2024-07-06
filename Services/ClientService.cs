using DTOModel.ClientDTO;
using DomainModel.DB;
using DataAccessLayerDB;
using Microsoft.EntityFrameworkCore;
using DTOModel;
using System.Diagnostics;

namespace Services
{
    public class ClientService
    {
        private readonly PSDBContext _dbContext;
        public ClientService(PSDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<ClientDTO> GetAllClientsByUserId(string type,Guid userId)
        {
            if(type == "cred")
            {

                UserDBDM user = _dbContext.Users.Include(u => u.teams)
                                                           .Include(u => u.teams).ThenInclude(t => t.client)
                                                           .Include(u => u.teams).ThenInclude(t => t.credentials).Single(u => u.id == userId);
                
                

                return ConvertClientDBDMListToClientDTOList(user.teams.Where(t => t.credentials.Count > 0).Select(t => t.client).ToList());

            }
            else if(type == "cert")
            {
                UserDBDM user = _dbContext.Users.Include(u => u.teams)
                                                           .Include(u => u.teams).ThenInclude(t => t.client)
                                                           .Include(u => u.teams).ThenInclude(t => t.certificates).Single(u => u.id == userId);

                return ConvertClientDBDMListToClientDTOList(user.teams.Where(t => t.certificates.Count > 0).Select(t => t.client).ToList());
            }
            else
            {
                return new List<ClientDTO>();
            }
        }
    
        public List<ClientDBDM> GetAllClients()
        {
            return _dbContext.Clients.ToList();
        }

        public List<ClientDTO> ConvertClientDBDMListToClientDTOList(List<ClientDBDM> clients)
        {
            List<ClientDTO> output = new List<ClientDTO>();
            foreach(ClientDBDM client in clients)
            {
                output.Add(new ClientDTO()
                {
                    name = client.name,
                    id = client.id,
                    createdate = client.createdate,
                    updatedate = client.updatedate,
                });
            }
            return output;
        }

        public List<ClientDTO> ConvertClientDBDMListToClientPartDTOList(List<ClientDBDM> clients)
        {
            List<ClientDTO> output = new List<ClientDTO>();
            foreach (ClientDBDM client in clients)
            {
                output.Add(new ClientDTO()
                {
                    name = client.name,
                    id = client.id,
                    createdate = null,
                    updatedate = null
                });
            }
            return output;
        }

        public SetStatus AddNewClient(PostClient postClient)
        {
            try
            {
                _dbContext.Clients.Add(new ClientDBDM()
                {
                    id = Guid.NewGuid(),
                    name = postClient.name,
                    createdate = DateTime.Now,
                    updatedate = DateTime.Now,
                });
                _dbContext.SaveChanges();
                return new SetStatus() { status = "OK" };
            }
            catch
            {
                return new SetStatus() { status = "KO" };
            }

        }
    }
}
