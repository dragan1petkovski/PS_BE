using DTOModel.TeamDTO;
using DomainModel;
using DataAccessLayerDB;
using Microsoft.EntityFrameworkCore;
using DTOModel;
using DataMapper;
using AuthenticationLayer;
using DTOModel.UserDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;


namespace Services
{
    public class TeamService
    {
        public List<TeamDTO> GetAllTeams(PSDBContext _dbContext, TeamDataMapper _dataMapper)
        {
            _dbContext.Teams.Include(t => t.client).ToList();
            return _dataMapper.ConvertTeamListToDTOList(_dbContext.Teams.Include(t => t.client).ToList());
        }
    
        public List<ClientTeamMapping> GetAllClientTeamMappingsByUserId([FromServices] PSDBContext _dbContext, [FromServices] TeamDataMapper _dataMapper, [FromServices] JwtTokenManager _jwtTokenManager, string jwt) 
        {
            if(_jwtTokenManager.GetUserID(jwt,out Guid userid))
            {
				List<ClientTeamMapping> output = new List<ClientTeamMapping>();

                //User splitQuery() to increase performance
                User user = _dbContext.Users.Include(u => u.teams)
                                                  .Include(u => u.teams)
                                                  .ThenInclude(t => t.client)
                                                  .FirstOrDefault(u => u.Id == userid.ToString());
                if(user != null)
                {
                    List<ClientTeamMapping> listOfTeams = user.teams.Select(t => new ClientTeamMapping() { teamid = t.id, clientid = t.client.id, teamname = t.name, clientname = t.client.name }).ToList();
					foreach (var team in listOfTeams)
					{
						output.Add(team);
					}

					return output;
				}
								


			}
            return new List<ClientTeamMapping>();
        }

        public List<ClientTeamMapping> GetAllClientTeamMappings([FromServices] PSDBContext _dbContext)
        {
            return _dbContext.Teams.Include(t => t.client).Select(t => new ClientTeamMapping(){
                teamname = t.name,
                teamid = t.id,
                clientid = t.client.id,
                clientname = t.client.name,
            }).ToList();
        }


        public SetStatus Create(PostTeamDTO _newTeam, [FromServices] PSDBContext _dbContext)
        {
            try
            {
                Team newTeam = new Team();
                newTeam.id = Guid.NewGuid();
                newTeam.name = _newTeam.name;
                newTeam.client = _dbContext.Clients.Find(_newTeam.clientid);
                newTeam.users = _dbContext.Users.Where(u => _newTeam.userids.Any(tu => tu.ToString() == u.Id)).ToList();
                newTeam.updatedate = DateTime.Now;
                newTeam.createdate = DateTime.Now;

                _dbContext.Teams.Add(newTeam);
                _dbContext.SaveChanges();
                return new SetStatus() { status = "OK" };
            }
            catch
            {
                return new SetStatus() { status = "KO" };
            }

        }
    
        public SetStatus Update(PostTeamUpdate update, [FromServices] PSDBContext _dbContext)
        {
            Team team = _dbContext.Teams.Include(t => t.users).FirstOrDefault(t => t.id == update.Id);
            if(team != null)
            {
                if(update.userIds.Count > 0)
                {
                    team.name = update.name;
					team.users.RemoveAll(u => true);
					foreach (Guid id in update.userIds)
					{
						User user = _dbContext.Users.Find(id.ToString());
						if (user != null)
						{
							team.users.Add(user);
						}
					}
				}
                else
                {
					team.users.RemoveAll(u => true);
				}
                team.updatedate = DateTime.Now;
				_dbContext.Teams.Update(team);
				_dbContext.SaveChanges();
				return new SetStatus() { status = "OK" };
			}
            return new SetStatus() { status = "KO" };
        }

		public async Task<GetTeamUpdate> Update([FromServices] PSDBContext _dbContext, Guid teamid, [FromServices] UserDataMapper _dataMapper, [FromServices] UserManager<User> _userManager)
		{
            GetTeamUpdate update = new GetTeamUpdate();
			Team team = _dbContext.Teams.Include(t => t.users).FirstOrDefault(t => t.id == teamid);
			if (team != null)
			{
                update.Id = team.id;
                update.name = team.name;
				update.users= _dataMapper.ConvertUserListToUserPartDTOList(await _dataMapper.GetUsersWithRoleAsync(_dbContext, _userManager, "User", team.users));
				return update;
			}
            return null;

			
		}
	
    
        public SetStatus Delete(Guid teamid, [FromServices] CertificateService _certificateService, [FromServices] PSDBContext _dbContext,[FromServices] IConfiguration _configuration)
        {
            Team teamToRemove = _dbContext.Teams.Include(t => t.users)
                                                .Include(t => t.credentials)
                                                .Include(t => t.credentials).ThenInclude(c => c.password)
                                                .Include(t => t.certificates).ThenInclude(c => c.password)
												.Include(t => t.certificates).ThenInclude(c => c.file)
												.Include(t => t.certificates).ThenInclude(c => c.key)
												.FirstOrDefault(t => t.id == teamid);
			
            List<Password> passwordsToRemove = new List<Password>();
            List<Credential> credentialToRemove = new List<Credential>();
            List<Certificate> certificateToRemove = new List<Certificate>();
            List<CertificateFile> fileToRemove = new List<CertificateFile>();

            if (teamToRemove != null)
            {

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

                _dbContext.SaveChanges();
                return new SetStatus() { status = "OK" };
            }
            else
            {
                return new SetStatus() { status = "KO" };
            }
        }
    }
}
