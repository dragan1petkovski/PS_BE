﻿using DTO.Team;
using DomainModel;
using DataAccessLayerDB;
using Microsoft.EntityFrameworkCore;
using DTO;
using DataMapper;
using DTO.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using LogginMessages;


namespace AppServices
{
    public class TeamService
    {
        public (StatusMessages statusCode, List<DTO.Team.Team> output) GetAllTeams(PSDBContext _dbContext, TeamDataMapper _dataMapper)
        {
			try
			{
				return (StatusMessages.Ok,_dataMapper.ConvertTeamListToDTOList(_dbContext.Teams.Include(t => t.client).ToList()));
			}
            catch
			{
				return (StatusMessages.UnableToService, null);
			}
        }
    
        public (StatusMessages statusCode, List<ClientTeamMapping> output) GetAllClientTeamMappingsByUserId(Guid userid, [FromServices] PSDBContext _dbContext, [FromServices] TeamDataMapper _dataMapper) 
        {
			List<ClientTeamMapping> output = new List<ClientTeamMapping>();
			DomainModel.User user = null;
			try
			{
				user = _dbContext.Users.Include(u => u.teams).AsSplitQuery()
											.Include(u => u.teams).ThenInclude(t => t.client).AsSplitQuery()
											.FirstOrDefault(u => u.Id == userid.ToString());
			}
			catch
			{
				return (StatusMessages.UnableToService, null);
			}

			if (user == null)
			{
				return (StatusMessages.AccessDenied, null);
			}

			List<ClientTeamMapping> listOfTeams = user.teams.Select(t => new ClientTeamMapping() { teamid = t.id, clientid = t.client.id, teamname = t.name, clientname = t.client.name }).ToList();
			foreach (var team in listOfTeams)
			{
				output.Add(team);
			}

			return (StatusMessages.Ok,output);

		}

		public (StatusMessages statusCode, List<ClientTeamMapping> output) GetAllClientTeamMappings([FromServices] PSDBContext _dbContext)
        {
			try
			{
				return (StatusMessages.Ok,_dbContext.Teams.Include(t => t.client).Select(t => new ClientTeamMapping()
				{
					teamname = t.name,
					teamid = t.id,
					clientid = t.client.id,
					clientname = t.client.name,
				}).ToList());
			}
			catch
			{
				return(StatusMessages.UnableToService,null);
			}
        }

        public StatusMessages Create(PostTeam _newTeam, [FromServices] PSDBContext _dbContext)
        {
            try
            {
				DomainModel.Team newTeam = new DomainModel.Team();
                newTeam.id = Guid.NewGuid();
                newTeam.name = _newTeam.name;
                newTeam.client = _dbContext.Clients.Find(_newTeam.clientid);
                newTeam.users = _dbContext.Users.Where(u => _newTeam.userids.Any(tu => tu.ToString() == u.Id)).ToList();
                newTeam.updatedate = DateTime.Now;
                newTeam.createdate = DateTime.Now;

                _dbContext.Teams.Add(newTeam);
                _dbContext.SaveChanges();
                return StatusMessages.AddNewTeam;
            }
            catch
            {
                return StatusMessages.FailedToAddTeam;
            }

        }
    
        public StatusMessages Update(PostTeamUpdate update, [FromServices] PSDBContext _dbContext)
        {
			DomainModel.Team team = _dbContext.Teams.Include(t => t.users).FirstOrDefault(t => t.id == update.Id);
            if(team == null)
            {
				return StatusMessages.TeamNotexist;
			}
            
			if (update.userIds.Count > 0)
			{
				team.name = update.name;
				team.users.RemoveAll(u => true);
				foreach (Guid id in update.userIds)
				{
					DomainModel.User user = _dbContext.Users.Find(id.ToString());
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
			return StatusMessages.UpdateTeam;
		}

		//Get Team do update
		public async Task<(StatusMessages,GetTeamUpdate)> Update([FromServices] PSDBContext _dbContext, Guid teamid, [FromServices] UserDataMapper _dataMapper, [FromServices] UserManager<DomainModel.User> _userManager)
		{
            GetTeamUpdate update = new GetTeamUpdate();
			DomainModel.Team team = _dbContext.Teams.Include(t => t.users).FirstOrDefault(t => t.id == teamid);
			if (team == null)
			{
				return (StatusMessages.TeamNotexist,null);
			}
            
			update.Id = team.id;
			update.name = team.name;
			update.users = _dataMapper.ConvertUserListToUserPartDTOList(await _dataMapper.GetUsersWithRoleAsync(_dbContext, _userManager, "User", team.users));
			return (StatusMessages.Ok,update);

		}
	
    
        public StatusMessages Delete(Guid teamid, [FromServices] CertificateService _certificateService, [FromServices] PSDBContext _dbContext,[FromServices] IConfiguration _configuration)
        {
			DomainModel.Team teamToRemove = _dbContext.Teams.Include(t => t.users)
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

            if (teamToRemove == null)
            {
				return StatusMessages.TeamNotexist;
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

			_dbContext.SaveChanges();
			return StatusMessages.DeleteTeam;
		}
    }
}
