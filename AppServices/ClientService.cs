using DTO.Client;
using DomainModel;
using DataAccessLayerDB;
using Microsoft.EntityFrameworkCore;
using DTO;
using DataMapper;
using DTO.Team;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using LogginMessages;

namespace AppServices
{
    public class ClientService
    {
        public (StatusMessages, List<ClientForUsers>) GetCredentialClientsByUserId(PSDBContext _dbContext,ClientDataMapper clientDataMapper, Guid userid)
        {
            User user = null;
            try
            {
                user = _dbContext.Users.Include(u => u.teams).AsSplitQuery()
                                        .Include(u => u.teams).ThenInclude(t => t.client).AsSplitQuery()
                                        .Include(u => u.teams).ThenInclude(t => t.credentials).FirstOrDefault(u => u.Id == userid.ToString());
            }
            catch
            {
                return(StatusMessages.UnableToService,null);
            }
            if(user == null)
            {
                return (StatusMessages.UnauthorizedAccess, null);
            }
			return (StatusMessages.Ok,clientDataMapper.ConvertClientListToClientDTOListForUsers(user.teams.Where(t => t.credentials.Count > 0).Select(t => t.client).Distinct().ToList()));

		}

        public (StatusMessages, List<ClientForUsers>) GetCertificateClientsByUserId(PSDBContext _dbContext,ClientDataMapper clientDataMapper, Guid userid)
        {
            User user = null;
            try
            {
				user = _dbContext.Users.Include(u => u.teams).AsSplitQuery()
									  .Include(u => u.teams).ThenInclude(t => t.client).AsSplitQuery()
									  .Include(u => u.teams).ThenInclude(t => t.certificates).Single(u => u.Id == userid.ToString());
			}
            catch
            {
                return (StatusMessages.UnableToService,null);
            }

            if(user ==null)
            {
                return (StatusMessages.UnauthorizedAccess, null);
            }
			return (StatusMessages.Ok, clientDataMapper.ConvertClientListToClientDTOListForUsers(user.teams.Where(t => t.certificates.Count > 0).Select(t => t.client).Distinct().ToList()));
			
		}

        public (StatusMessages, List<ClientForAdmins>) GetAllClientsForAdmin(PSDBContext _dbContext,ClientDataMapper dataMapper)
        {
			List<DomainModel.Client> clients = null;
			try
			{
				clients = _dbContext.Clients.ToList();
			}
			catch
			{
				return (StatusMessages.UnableToService, null);
			}
			return (StatusMessages.Ok, dataMapper.ConvertClientListToClientDTOListForAdmins(clients));
		}

		public (StatusMessages, List<ClientForUsers>) GetAllClientsForUser(PSDBContext _dbContext, ClientDataMapper dataMapper)
        {
            List<DomainModel.Client> clients = null;
            try
            {
                clients = _dbContext.Clients.ToList();
			}
            catch
            {
                return (StatusMessages.UnableToService,null);
            }
            return (StatusMessages.Ok,dataMapper.ConvertClientListToClientDTOListForUsers(clients));
        }

        public (StatusMessages,ClientForAdmins) Create(PSDBContext _dbContext,PostClient postClient)
        {
            try
            {
                Client newClient = new Client()
                {
                    id = Guid.NewGuid(),
                    name = postClient.name,
                    createdate = DateTime.Now,
                    updatedate = DateTime.Now,
                };
                _dbContext.Clients.Add(newClient);
				_dbContext.SaveChanges();
                ClientForAdmins output = new ClientForAdmins();
                output.id = newClient.id;
                output.name = newClient.name;
                output.createdate = DateTime.Now;

                return (StatusMessages.AddNewClient,output);
            }
            catch
            {
                return (StatusMessages.UnableToService,null);
            }

        }
    
        public (StatusMessages,ClientUpdate) Update(Guid id, PSDBContext _dbContext)
        {
            Client client = null;
            try
            {
				client = _dbContext.Clients.Find(id);
			}
            catch
            {
                return (StatusMessages.UnableToService,null);
            }
            if (client == null)
            {
                return (StatusMessages.ClientNotexist,null);
            }
			return (StatusMessages.Ok, new ClientUpdate() { id = client.id, name = client.name });
		}

        public StatusMessages Update(ClientUpdate update, PSDBContext _dbContext)
        {
            Client client = null;
            try
            {
				client = _dbContext.Clients.Find(update.id);
			}
            catch
            {
                return StatusMessages.UnableToService;
            }
            if (client == null)
            {
                return StatusMessages.ClientNotexist;
			}
			client.name = update.name;
			client.updatedate = DateTime.Now;
			_dbContext.Clients.Update(client);
			_dbContext.SaveChanges(true);
			return StatusMessages.UpdateClient;
		}

        //Missing Logging features
        public StatusMessages Delete(Guid clientid, [FromServices] TeamService _teamService, [FromServices] CertificateService _certificateService, [FromServices] PSDBContext _dbContext, [FromServices] IConfiguration _configuration)
        {
            Client client = null;
            try
            {
				client = _dbContext.Clients.Include(c => c.teams).FirstOrDefault(c => c.id == clientid);
			}
            catch
            {
                return StatusMessages.UnableToService;
            }
            if (client == null)
            {
				return StatusMessages.ClientNotexist;
			}
			List<Guid> teams = client.teams.Select(t => t.id).ToList();
			if (teams == null)
			{
				return StatusMessages.TeamNotexist;
			}

            foreach(Guid teamid in teams)
            {
				DomainModel.Team teamToRemove = _dbContext.Teams.Include(t => t.users)
													.Include(t => t.credentials).AsSplitQuery()
													.Include(t => t.credentials).ThenInclude(c => c.password).AsSplitQuery()
													.Include(t => t.certificates).ThenInclude(c => c.password).AsSplitQuery()
													.Include(t => t.certificates).ThenInclude(c => c.file).AsSplitQuery()
													.Include(t => t.certificates).ThenInclude(c => c.key).AsSplitQuery()
													.FirstOrDefault(t => t.id == teamid);

				List<Password> passwordsToRemove = new List<Password>();
				List<Credential> credentialToRemove = new List<Credential>();
				List<Certificate> certificateToRemove = new List<Certificate>();
				List<CertificateFile> fileToRemove = new List<CertificateFile>();

				if (teamToRemove == null)
				{
                    continue;
				}

				teamToRemove.users.RemoveAll(t => true);

				passwordsToRemove.AddRange(teamToRemove.credentials.Select(c => c.password).ToList());
				passwordsToRemove.AddRange(teamToRemove.certificates.Where(c => c.password != null).Select(c => c.password).ToList());

				fileToRemove.AddRange(teamToRemove.certificates.Select(c => c.file).ToList());
				fileToRemove.AddRange(teamToRemove.certificates.Where(c => c.key != null).Select(c => c.key).ToList());
				teamToRemove.certificates.ForEach(c => _certificateService.DeleteCertificateFile(_configuration, c.file.currentFileName));
				teamToRemove.certificates.Where(c => c.key != null).ToList().ForEach(c => _certificateService.DeleteCertificateFile(_configuration, c.key.currentFileName));

				credentialToRemove.AddRange(teamToRemove.credentials);
				certificateToRemove.AddRange(teamToRemove.certificates);


				_dbContext.Passwords.RemoveRange(passwordsToRemove);
				_dbContext.CertificatesFile.RemoveRange(fileToRemove);
				_dbContext.Certificates.RemoveRange(certificateToRemove);
				_dbContext.Credentials.RemoveRange(credentialToRemove);
				_dbContext.Teams.Remove(teamToRemove);

			}


			_dbContext.Clients.Remove(client);
			_dbContext.SaveChanges(true);
			return StatusMessages.DeleteClient;


		}
    }
}
