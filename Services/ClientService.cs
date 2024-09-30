using DTOModel.ClientDTO;
using DomainModel;
using DataAccessLayerDB;
using Microsoft.EntityFrameworkCore;
using DTOModel;
using DataMapper;
using AuthenticationLayer;
using Microsoft.Identity.Client;
using DTOModel.TeamDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Services
{
    public class ClientService
    {
        public List<ClientDTOForUsers> GetCredentialClientsByUserId(PSDBContext _dbContext,ClientDataMapper clientDataMapper, JwtTokenManager jwtTokenManager, string jwt)
        {
            if(jwtTokenManager.GetUserID(jwt,out Guid userid))
            {
				User user = _dbContext.Users.Include(u => u.teams)
														   .Include(u => u.teams).ThenInclude(t => t.client)
														   .Include(u => u.teams).ThenInclude(t => t.credentials).Single(u => u.Id == userid.ToString());



				return clientDataMapper.ConvertClientListToClientDTOListForUsers(user.teams.Where(t => t.credentials.Count > 0).Select(t => t.client).Distinct().ToList());
			}
			return new List<ClientDTOForUsers>();

		}

        public List<ClientDTOForUsers> GetCertificateClientsByUserId(PSDBContext _dbContext,ClientDataMapper clientDataMapper, JwtTokenManager jwtTokenManager, string jwt)
        {
            if (jwtTokenManager.GetUserID(jwt, out Guid userid))
            {
				User user = _dbContext.Users.Include(u => u.teams)
														   .Include(u => u.teams).ThenInclude(t => t.client)
														   .Include(u => u.teams).ThenInclude(t => t.certificates).Single(u => u.Id == userid.ToString());

				return clientDataMapper.ConvertClientListToClientDTOListForUsers(user.teams.Where(t => t.certificates.Count > 0).Select(t => t.client).Distinct().ToList());
			}
			return new List<ClientDTOForUsers>();
		}

		public List<Client> GetAllClients(PSDBContext _dbContext)
        {
            return _dbContext.Clients.ToList();
        }

        public SetStatus AddNewClient(PSDBContext _dbContext,PostClient postClient)
        {
            try
            {
                _dbContext.Clients.Add(new Client()
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
    
        public ClientUpdate Update(Guid id, PSDBContext _dbContext)
        {
            Client client = _dbContext.Clients.Find(id);
            if (client != null)
            {
                return new ClientUpdate() { id = client.id, name = client.name };
            }
            return null;
        }

        public SetStatus Update(ClientUpdate update, PSDBContext _dbContext)
        {
            Client client = _dbContext.Clients.Find(update.id);
            if (client != null)
            {
                client.name = update.name;
                client.updatedate = DateTime.Now;
                _dbContext.Clients.Update(client);
                _dbContext.SaveChanges(true);
				return new SetStatus() { status = "OK" };
			}
			return new SetStatus() { status = "KO" };
		}

        public SetStatus Delete(Guid clientid, [FromServices] TeamService _teamService, [FromServices] CertificateService _certificateService, [FromServices] PSDBContext _dbContext, [FromServices] IConfiguration _configuration)
        {
            Client client = _dbContext.Clients.Include(c => c.teams).FirstOrDefault(c => c.id == clientid);
            if (client != null)
            {
				List<Team> teams = client.teams.ToList();
                if(teams != null)
                {
                    foreach (Team team in teams)
                    {
                        ClientTeamPair pair = new ClientTeamPair() { clientid = client.id, teamid = team.id };
                        if(_teamService.Delete(pair.teamid, _certificateService, _dbContext, _configuration).status == "KO")
                        {
                            return new SetStatus() { status = "KO" };
                        }
                        
                        
                    }
                    _dbContext.Clients.Remove(client);
                    _dbContext.SaveChanges(true);
					return new SetStatus() { status = "OK" };
				}
				return new SetStatus() { status = "KO" };
			}
            
            return new SetStatus() { status = "KO" };
        }
    }
}
