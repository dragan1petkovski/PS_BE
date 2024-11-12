﻿using DTO.Credential;
using DTO;
using DomainModel;
using DataAccessLayerDB;
using Microsoft.EntityFrameworkCore;
using TransitionObjectMapper;
using Microsoft.AspNetCore.Mvc;
using DataMapper;
using EncryptionLayer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using LogginMessages;
using System.Collections.Immutable;
using DTO.Personal;
using Org.BouncyCastle.Asn1.X509;

namespace AppServices
{
    public  class CredentialService
    {
		//Return List of Credentials that belong to Same User in Default Personal Folder
		public (StatusMessages statusCode, List<PersonalCredential> output) GetPersonalCredentialsByUserId(Guid userid,PSDBContext dbContext)
		{
			List<PersonalCredential> personalCredentialList = new List<PersonalCredential>();
			DomainModel.User user = null;
			try
			{
				user = dbContext.Users.Include(u => u.credentials).FirstOrDefault(u => u.Id == userid.ToString());
			}
			catch
			{
				Console.WriteLine($"User does not exist");
				return (StatusMessages.UnableToService, null);
			}
			foreach (DomainModel.Credential credential in user.credentials)
			{
				personalCredentialList.Add(new PersonalCredential()
				{
					id = credential.id,
					domain = credential.domain,
					username = credential.username,
					email = credential.email,
					remote = credential.remote,
					password = "*****",
					note = credential.note
				});
			}
			return (StatusMessages.Ok,personalCredentialList);
		}
		
		//Return List of Credentials that belong to same User and to same Personal Folder
		public (StatusMessages statusCode, List<PersonalCredential> output) GetPersonalCredentialsByFolderId(Guid userid, Guid folderid, PSDBContext dbContext)
		{
			List<PersonalCredential> personalCredentialList = new List<PersonalCredential>();

			DomainModel.User user = null;
			DomainModel.PersonalFolder folder = null;
			try
			{
				user = dbContext.Users.Include(u => u.folders).ThenInclude(u => u.credentials).First(u => u.Id == userid.ToString() && u.folders.Any(pf => pf.id == folderid));
			}	
			catch
			{
				Console.WriteLine($"User does not exist");
				return (StatusMessages.UnableToService, null);
			}

			folder = user.folders.Find(pf => pf.id == folderid);
			foreach (DomainModel.Credential credential in folder.credentials)
			{
				personalCredentialList.Add(new PersonalCredential()
				{
					id = credential.id,
					domain = credential.domain,
					username = credential.username,
					email = credential.email,
					remote = credential.remote,
					password = "*****",
					note = credential.note,
					personalfolderid = folderid
				});
			}
			return (StatusMessages.Ok,personalCredentialList);
		}

		//Return List of Credentials that belong to same Client
		public (StatusMessages statusCode , List<DTO.Credential.Credential> output) GetClientCredentialsByClientId(Guid userid, Guid clientid, PSDBContext dbContext, CredentialDataMapper dataMapper)
		{
			DomainModel.User user = null;
			List<TeamCredentialsMap> teams = null;
			try
			{
				//User splitQuery() to increase performance
				user = dbContext.Users.Include(u => u.teams)
										   .Include(u => u.teams).ThenInclude(t => t.client).AsSplitQuery()
										   .Include(u => u.teams).ThenInclude(t => t.credentials).AsSplitQuery()
										   .FirstOrDefault(u => u.Id == userid.ToString());
				
			}
			catch
			{
				return (StatusMessages.UnableToService, null);
			}
			if(user == null)
			{
				return (StatusMessages.UnauthorizedAccess, null);
			}

			teams = user.teams.Where(t => t.client.id == clientid)
							  .Select(t => new TeamCredentialsMap() { teamid = t.id, teamname = t.name, credentials = t.credentials, clientid = clientid }).ToList();

			return (StatusMessages.Ok, dataMapper.ConvertToCredentialDTO(teams));
		}

		//Return Single Credential that belong to Client
		public (StatusMessages statusCode, DTO.Credential.Credential output) GetClientCredentialByCredentialId(Guid userid, Guid teamid, Guid credentialid, PSDBContext dbContext, CredentialDataMapper dataMapper)
		{
			DomainModel.User user = null;
			DomainModel.Team team = null;
			DomainModel.Credential credential = null;
			try
			{

				user = dbContext.Users.Include(u => u.teams)
									  .Include(u => u.teams).ThenInclude(t => t.credentials).AsSplitQuery()
									  .FirstOrDefault(u => u.Id == userid.ToString() && u.teams.Any(t => t.id == teamid) && u.teams.Any(t => t.credentials.Any(c => c.id == credentialid)));
											
				
				
			}
			catch
			{
				return (StatusMessages.UnableToService,null);
			}

			if(user == null)
			{
				return (StatusMessages.UnauthorizedAccess, null);
			}

			team = user.teams.First(t => t.id == teamid);		
			if (team == null)
			{
				return (StatusMessages.TeamNotexist, null);
			}

			credential = team.credentials.Find(c => c.id == credentialid);
			if (credential == null)
			{
				return (StatusMessages.CredentialNotexist, null);
			}

			return (StatusMessages.Ok, dataMapper.ConvertToCredentialDTO(credential));
		}

		//Return Single Personal Credential that belong to user
		public (StatusMessages statusCode, PersonalCredential output) GetPersonalCredentialByCredentialId(Guid userid, Guid credentialid, PSDBContext dbContext)
		{
			DomainModel.User user = null;
			try
			{
				user = dbContext.Users.Include(u => u.credentials).FirstOrDefault(u => u.Id == userid.ToString());
			}
			catch
			{
				return (StatusMessages.UnableToService,null);
			}
			if(user ==null)
			{
				return (StatusMessages.UnauthorizedAccess, null);
			}
			
			DomainModel.Credential credential = user.credentials.FirstOrDefault(c => c.id == credentialid);
			if(credential == null)
			{
				return (StatusMessages.CredentialNotexist,null);
			}

			return (StatusMessages.Ok, new PersonalCredential()
			{
				id = credential.id,
				domain = credential.domain,
				username = credential.username,
				email = credential.email,
				remote = credential.remote,
				password = "*****",
				note = credential.note
			});
		}

		//Return Single Personal Credential that belong to User in specific folder
		public (StatusMessages statusCode, PersonalCredential output) GetPersonalCredentialByCredentialId(Guid userid, Guid personalfolderid, Guid credentialid, PSDBContext dbContext)
		{
			DomainModel.User user = null;
			try
			{
				user = dbContext.Users.Include(u => u.folders)
									  .Include(u => u.folders).ThenInclude(u => u.credentials)
									  .FirstOrDefault(u => u.Id == userid.ToString());
			}
			catch
			{
				Console.WriteLine($"User does not exist");
				return (StatusMessages.CredentialNotexist,null);
			}

			if(user == null)
			{
				return (StatusMessages.UnauthorizedAccess,null);
			}

			DomainModel.PersonalFolder folder = user.folders.FirstOrDefault(f => f.id == personalfolderid);
            if (folder == null)
            {
				return (StatusMessages.PersonalFolderNotexist, null);
            }

            DomainModel.Credential credential = folder.credentials.FirstOrDefault(c => c.id == credentialid);
			if (credential == null)
			{
				return (StatusMessages.CredentialNotexist,null);
			}

			return (StatusMessages.Ok, new PersonalCredential()
			{
				id = credential.id,
				domain = credential.domain,
				username = credential.username,
				email = credential.email,
				remote = credential.remote,
				password = "*****",
				note = credential.note
			});
		}
		
		public StatusMessages CreateCredential(Guid userid,PostCredential postCredential, PSDBContext _dbContext, SymmetricEncryption symmetricEncryption, IConfiguration configuration, ILogger _logger)
        {
			User user = null;
			try
			{
				user = _dbContext.Users.Include(u => u.teams)
								.Include(u => u.teams).ThenInclude(t => t.credentials).AsSplitQuery()
								.Include(u => u.teams).ThenInclude(t => t.client).AsSplitQuery()
								.FirstOrDefault(u => u.Id == userid.ToString());
			}
			catch 
			{
				return StatusMessages.UnableToService;
			}
			if(user == null)
			{
				return StatusMessages.UnauthorizedAccess;
			}

			List<Team> teams= user.teams.Where(t => postCredential.teams.Any(team => team.teamid == t.id && team.clientid == t.client.id)).ToList();
			if (teams == null)
			{
				return StatusMessages.TeamNotexist;
			}
			teams.ForEach(t =>
			{
				DomainModel.Credential newCredential = new DomainModel.Credential();
				SymmetricKey key = symmetricEncryption.EncryptString(postCredential.password, configuration);

				newCredential.id = Guid.NewGuid();
				newCredential.domain = postCredential.domain;
				newCredential.password = new Password() { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };
				newCredential.username = postCredential.username;
				newCredential.email = postCredential.email;
				newCredential.remote = postCredential.remote;
				newCredential.note = postCredential.note;
				newCredential.createdate = DateTime.Now;
				newCredential.updatedate = DateTime.Now;

				_dbContext.Passwords.Add(newCredential.password);
				_dbContext.Credentials.Add(newCredential);

				if (t.credentials != null)
				{
					t.credentials.Add(newCredential);
					_dbContext.Teams.Update(t);
				}
				else
				{
					t.credentials = [newCredential];
					_dbContext.Teams.Update(t);
				}

			});

			try
			{
				_dbContext.SaveChanges();
				return StatusMessages.AddNewCredential;
			}
			catch
			{
				return StatusMessages.UnableToService;
			}
			

		}

		public StatusMessages CreatePersonalCredential(Guid userid, PostPersonalCredential postPersonalCredential, PSDBContext _dbContext, SymmetricEncryption _symmetricEncryption, IConfiguration _configuration)
		{
			User user = _dbContext.Users.Include(u => u.folders)
										.Include(u => u.folders).ThenInclude(pf => pf.credentials)
										.FirstOrDefault(u => u.Id == userid.ToString());

			if (user == null)
			{
				return StatusMessages.UnauthorizedAccess;
			}

			DomainModel.PersonalFolder personalFolder = user.folders.FirstOrDefault(pf => pf.id == postPersonalCredential.personalFolderId);
			if (personalFolder != null)
			{
				DomainModel.Credential newPersonalCredential = new DomainModel.Credential();
				newPersonalCredential.domain = postPersonalCredential.domain;
				newPersonalCredential.username = postPersonalCredential.username;

				SymmetricKey key = _symmetricEncryption.EncryptString(postPersonalCredential.password, _configuration);

				newPersonalCredential.password = new Password() { password = key.password, aad = key.aad, id = Guid.NewGuid(), createdate = DateTime.Now, updatedate = DateTime.Now };
				newPersonalCredential.remote = postPersonalCredential.remote;
				newPersonalCredential.email = postPersonalCredential.email;
				newPersonalCredential.note = postPersonalCredential.note;


				if (personalFolder.credentials != null)
				{
					personalFolder.credentials.Add(newPersonalCredential);
				}
				else
				{
					personalFolder.credentials = [newPersonalCredential];
				}

				try
				{
					_dbContext.Passwords.Add(newPersonalCredential.password);
					_dbContext.Credentials.Add(newPersonalCredential);
					_dbContext.PersonalFolders.Update(personalFolder);

					_dbContext.SaveChanges();
					return StatusMessages.AddNewCredential;
				}
				catch
				{
					return StatusMessages.UnableToService;
				}
			}
			else
			{
				DomainModel.Credential newPersonalCredential = new DomainModel.Credential();
				newPersonalCredential.domain = postPersonalCredential.domain;
				newPersonalCredential.username = postPersonalCredential.username;

				SymmetricKey key = _symmetricEncryption.EncryptString(postPersonalCredential.password, _configuration);

				newPersonalCredential.password = new Password() { password = key.password, aad = key.aad, id = Guid.NewGuid(), createdate = DateTime.Now, updatedate = DateTime.Now };

				newPersonalCredential.remote = postPersonalCredential.remote;
				newPersonalCredential.email = postPersonalCredential.email;
				newPersonalCredential.note = postPersonalCredential.note;

				_dbContext.Passwords.Add(newPersonalCredential.password);
				_dbContext.Credentials.Add(newPersonalCredential);
				if (user.credentials != null)
				{
					user.credentials.Add(newPersonalCredential);
				}
				else
				{
					user.credentials = [newPersonalCredential];
				}
				try
				{
					_dbContext.Users.Update(user);
					_dbContext.SaveChanges();
					return StatusMessages.AddNewCredential;
				}
				catch
				{
					return StatusMessages.UnableToService;
				}
			}

		}

		public StatusMessages DeleteCredential(Guid id, Guid teamid, Guid userid, PSDBContext _dbContext, ILogger<CredentialService> _logger)
        {
			User user = null;
			try
			{
				user = _dbContext.Users.Include(u => u.teams).AsSplitQuery()
															.Include(u => u.teams).ThenInclude(t => t.credentials).AsSplitQuery()
															.Include(u => u.teams).ThenInclude(t => t.credentials).ThenInclude(c => c.password)
															.FirstOrDefault(u => u.Id == userid.ToString());
			}
			catch
			{
				return StatusMessages.UnableToService;
			}
			if(user == null)
			{
				return StatusMessages.UnauthorizedAccess;
			}

			Team team = user.teams.FirstOrDefault(t => t.id == teamid);
			if(team == null)
			{
				return StatusMessages.TeamNotexist;
			}

			DomainModel.Credential credential = team.credentials.FirstOrDefault(c => c.id == id);
			if(credential == null)
			{
				return StatusMessages.CredentialNotexist;
			}

			if(credential.password == null)
			{
				_logger.LogCritical($"Credential Orphan: {credential.id} - Requre manual delete beacuse Don't have Password in Database because Create or Delete process don't finish properly");
				return StatusMessages.CredentialNotexist;
			}

			_dbContext.Passwords.Remove(credential.password);
			_dbContext.Credentials.Remove(credential);
			_dbContext.SaveChanges();
			return StatusMessages.DeleteCredential;


		}

		public StatusMessages DeletePersonalCredential(Guid id, Guid personalfolderid, Guid userid, PSDBContext _dbContext, ILogger<CredentialService> _logger)
		{
			if (personalfolderid != Guid.Empty)
			{
				DomainModel.User user = null;
				try
				{
					user = _dbContext.Users.Include(u => u.folders)
												 .Include(u => u.folders).ThenInclude(t => t.credentials)
												 .Include(u => u.folders).ThenInclude(t => t.credentials).ThenInclude(c => c.password)
												 .FirstOrDefault(u => u.Id == userid.ToString());
				}
				catch
				{
					return StatusMessages.UnableToService;
				}
				if(user == null)
				{
					return StatusMessages.UnauthorizedAccess;
				}
				DomainModel.PersonalFolder personalFolder = user.folders.FirstOrDefault(pf => pf.id == personalfolderid);
				if(personalFolder == null)
				{
					return StatusMessages.PersonalFolderNotexist;
				}
				DomainModel.Credential credential = personalFolder.credentials.FirstOrDefault(c => c.id == id);
				if(credential == null)
				{
					return StatusMessages.CredentialNotexist;
				}
				_dbContext.Passwords.Remove(credential.password);
				_dbContext.Credentials.Remove(credential);
				_dbContext.SaveChanges();
				return StatusMessages.DeleteCredential;
			}
			else
			{
				DomainModel.User user = null;
				try
				{
					user = _dbContext.Users.Include(u => u.credentials)
												 .Include(u => u.credentials).ThenInclude(c => c.password)
												 .FirstOrDefault(u => u.Id == userid.ToString());
				}
				catch
				{
					return StatusMessages.UnableToService;
				}
				if(user == null)
				{
					return StatusMessages.UnauthorizedAccess;
				}
				
				DomainModel.Credential credential = user.credentials.FirstOrDefault(c => c.id == id);
				if(credential == null)
				{
					return StatusMessages.CredentialNotexist;
				}
				_dbContext.Passwords.Remove(credential.password);
				_dbContext.Credentials.Remove(credential);
				_dbContext.SaveChanges();
				return StatusMessages.DeleteCredential;
			}


		}

		public StatusMessages GiveCredential(PostGiveCredential _giveCredential, PSDBContext _dbContext, [FromServices] IConfiguration _configuration, [FromServices] SymmetricEncryption _symmetricEncryption)
        {
            try
            {
                DomainModel.Credential newCredential = new DomainModel.Credential();
                newCredential.id = Guid.NewGuid();
                newCredential.domain = _giveCredential.domain;
                newCredential.username = _giveCredential.username;

                SymmetricKey key = _symmetricEncryption.EncryptString(_giveCredential.password, _configuration);

                newCredential.password = new Password() { password = key.password, aad = key.aad, id = Guid.NewGuid(), createdate = DateTime.Now, updatedate = DateTime.Now };
                newCredential.email = _giveCredential.email;
                newCredential.createdate = DateTime.Now;
                newCredential.updatedate = DateTime.Now;
                newCredential.remote = _giveCredential.remote;
                newCredential.note = _giveCredential.note;

                _dbContext.Credentials.Add(newCredential);

                List<DomainModel.User> users = _dbContext.Users.Where(u => _giveCredential.userids.Any(uid => uid.ToString() == u.Id)).ToList();
                foreach (DomainModel.User user in users)
                {
                    if (user.credentials == null)
                    {
                        user.credentials = [newCredential];
                        
                    }
                    else
                    {
                        user.credentials.Add(newCredential);
                    }

                }

                List<Team> teams = _dbContext.Teams.Where(t => _giveCredential.teamids.Any(tid => tid == t.id)).ToList();
                foreach (Team team in teams)
                {
                    if (team.credentials == null)
                    {
                        team.credentials = [newCredential];
                        
                    }
                    else
                    {
                        team.credentials.Add(newCredential);
                    }
                }

                _dbContext.SaveChanges();
                return StatusMessages.GiveCredential;
            }
            catch
            {
                return StatusMessages.FailedToGiveCredential;
            }
        }
    
        public StatusMessages Update(PSDBContext _dbContext,Guid userid, PostUpdateCredential update, IConfiguration _configuration, SymmetricEncryption _symmetricEncryption)
        {
			DomainModel.User user = null;
			try
			{
				user = _dbContext.Users.Include(u => u.teams)
										.Include(u => u.teams).ThenInclude(t => t.credentials)
										.Include(u => u.teams).ThenInclude(t => t.credentials).ThenInclude(c => c.password)
										.FirstOrDefault(u => u.Id == userid.ToString());
			}
			catch
			{
				return StatusMessages.UnableToService;
			}

			if (user == null)
			{
				return StatusMessages.UnauthorizedAccess;
			}

			DomainModel.Team team = user.teams.FirstOrDefault(t => t.id == update.teamid);
			if (team == null)
			{
				return StatusMessages.AccessDenied;
			}

			DomainModel.Credential credential = team.credentials.FirstOrDefault(c => c.id == update.id);
			if (credential == null)
			{
				return StatusMessages.CredentialNotexist;
			}

			if (string.IsNullOrEmpty(update.password) || string.IsNullOrWhiteSpace(update.password))
            {

			    
				credential.email = update.email;
				credential.username = update.username;
				credential.domain = update.domain;
				credential.note = update.note;
				credential.remote = update.remote;
				credential.updatedate = DateTime.Now;

				try
				{
					_dbContext.Credentials.Update(credential);
					_dbContext.SaveChanges(true);
					return StatusMessages.UpdateCredential;
				}
				catch
				{
					return StatusMessages.UnableToService;
				}
			}
            else
            {
		
				credential.email = update.email;
				credential.username = update.username;
				credential.domain = update.domain;
				credential.note = update.note;
				credential.remote = update.remote;
				credential.updatedate = DateTime.Now;

				SymmetricKey key = _symmetricEncryption.EncryptString(update.password, _configuration);

				credential.password.aad = key.aad;
				credential.password.password = key.password;
				credential.updatedate = DateTime.Now;

				try
				{
					_dbContext.Passwords.Update(credential.password);
					_dbContext.Credentials.Update(credential);
					_dbContext.SaveChanges();
					return StatusMessages.UpdateCredential;
				}
				catch
				{
					return StatusMessages.UnableToService;
				}
			}
		}
    
        public StatusMessages UpdatePersonalCredential(PSDBContext _dbContext, PostUpdatePersonalCredential update, IConfiguration _configuration, SymmetricEncryption _symmetricEncryption, Guid _userid)
        {
            Guid.TryParse(update.personalfolderid, out Guid _personalfolderid);
            Guid.TryParse(update.originalpersonalfolderid, out Guid _originalpersonalfolderid);

            if(_originalpersonalfolderid == Guid.Empty)
            {
                if(_personalfolderid == Guid.Empty) 
                {
					DomainModel.Credential credential = new DomainModel.Credential();
					User user = new User();

					try
					{
						user = _dbContext.Users.Include(u => u.credentials)
												.Include(u => u.credentials).ThenInclude(c => c.password)
												.FirstOrDefault(u => u.Id == _userid.ToString());
					}
					catch
					{
						return StatusMessages.UnableToService;
					}

					if(user == null)
					{
						return StatusMessages.UnauthorizedAccess;
					}
					credential = user.credentials.FirstOrDefault(c => c.id == update.id);
					if(credential == null)
					{
						return StatusMessages.CredentialNotexist;
					}

					if (!string.IsNullOrEmpty(update.password) || !string.IsNullOrWhiteSpace(update.password))
                    {
						credential.domain = update.domain;
						credential.username = update.username;
						credential.email = update.email;
						credential.remote = update.remote;
						credential.note = update.note;

                        SymmetricKey key = _symmetricEncryption.EncryptString(update.password, _configuration);

                        credential.password.password = key.password;
                        credential.password.aad = key.aad;
                        credential.password.updatedate = DateTime.Now;

						user.credentials.Remove(credential);

						try
						{
							_dbContext.Users.Update(user);
							_dbContext.Passwords.Update(credential.password);
							_dbContext.Credentials.Update(credential);
							_dbContext.SaveChanges();
							return StatusMessages.UpdateCredential;
						}
						catch
						{
							return StatusMessages.UnableToService;
						}

					}
                    else
                    {

						credential.domain = update.domain;
						credential.username = update.username;
						credential.email = update.email;
						credential.remote = update.remote;
						credential.note = update.note;

						user.credentials.Remove(credential);

						try
						{
							_dbContext.Users.Update(user);
							_dbContext.Credentials.Update(credential);
							_dbContext.SaveChanges();
							return StatusMessages.UpdateCredential;
						}
						catch
						{
							return StatusMessages.UnableToService;
						}
					}
                }
                else
                {
					DomainModel.Credential credential = new DomainModel.Credential();
					DomainModel.PersonalFolder personalFolder = new DomainModel.PersonalFolder();
					DomainModel.User user = new DomainModel.User();

					try
					{
						user = _dbContext.Users.Include(u => u.credentials)
												.Include(u => u.credentials).ThenInclude(c => c.password).AsSplitQuery()
												.Include(u => u.folders)
												.FirstOrDefault(u => u.Id == _userid.ToString());


					}
					catch
					{
						return StatusMessages.UnableToService;
					}

					personalFolder = user.folders.FirstOrDefault(pf => pf.id == _personalfolderid);
					credential = user.credentials.FirstOrDefault(c => c.id == update.id);

					if (user == null)
					{
						return StatusMessages.UnauthorizedAccess;
					}
					
					if(personalFolder == null)
					{
						return StatusMessages.PersonalFolderNotexist;
					}

					if(credential == null)
					{
						return StatusMessages.CredentialNotexist;
					}

					if (!string.IsNullOrEmpty(update.password) || !string.IsNullOrWhiteSpace(update.password))
					{


						credential.domain = update.domain;
						credential.username = update.username;
						credential.email = update.email;
						credential.remote = update.remote;
						credential.note = update.note;

						SymmetricKey key = _symmetricEncryption.EncryptString(update.password, _configuration);

						credential.password.password = key.password;
						credential.password.aad = key.aad;
						credential.password.updatedate = DateTime.Now;

						if(personalFolder.credentials == null)
						{
							personalFolder.credentials = [credential];
						}
						else
						{
							personalFolder.credentials.Add(credential);
						}
                        

						user.credentials.Remove(credential);

						try
						{
							_dbContext.Users.Update(user);
							_dbContext.PersonalFolders.Update(personalFolder);
							_dbContext.Passwords.Update(credential.password);
							_dbContext.Credentials.Update(credential);
							_dbContext.SaveChanges();
							return StatusMessages.UpdateCredential;
						}
						catch
						{
							return StatusMessages.UnableToService;
						}

					}
					else
					{

						credential.domain = update.domain;
						credential.username = update.username;
						credential.email = update.email;
						credential.remote = update.remote;
						credential.note = update.note;

						if (personalFolder.credentials == null)
						{
							personalFolder.credentials = [credential];
						}
						else
						{
							personalFolder.credentials.Add(credential);
						}

						user.credentials.Remove(credential);

						try
						{
							_dbContext.Users.Update(user);
							_dbContext.PersonalFolders.Update(personalFolder);
							_dbContext.Credentials.Update(credential);
							_dbContext.SaveChanges();
							return StatusMessages.UpdateCredential;
						}
						catch
						{
							return StatusMessages.UnableToService;
						}
					}
				}
            }
            else
            {
				if (_personalfolderid == Guid.Empty)
				{

					DomainModel.Credential credential = new DomainModel.Credential();
					DomainModel.PersonalFolder originalPersonalFolder = new DomainModel.PersonalFolder();
					DomainModel.User user = new DomainModel.User();

					user = _dbContext.Users.Include(u => u.credentials).AsSplitQuery()
											.Include(u => u.folders)
											.Include(u => u.folders).ThenInclude(pf => pf.credentials)
											.Include(u => u.folders).ThenInclude(pf => pf.credentials).ThenInclude(c => c.password)
											.FirstOrDefault(u => u.Id == _userid.ToString());
					originalPersonalFolder = user.folders.FirstOrDefault(pf => pf.id == _originalpersonalfolderid);
					credential = originalPersonalFolder.credentials.FirstOrDefault(c => c.id == update.id);

					if(user == null)
					{
						return StatusMessages.UnauthorizedAccess;
					}
					if(originalPersonalFolder == null)
					{
						return StatusMessages.PersonalFolderNotexist;
					}
					if(credential == null)
					{
						return StatusMessages.CredentialNotexist;
					}

					if (!string.IsNullOrEmpty(update.password) || !string.IsNullOrWhiteSpace(update.password))
					{


						credential.domain = update.domain;
						credential.username = update.username;
						credential.email = update.email;
						credential.remote = update.remote;
						credential.note = update.note;

						SymmetricKey key = _symmetricEncryption.EncryptString(update.password, _configuration);

						credential.password.password = key.password;
						credential.password.aad = key.aad;
						credential.password.updatedate = DateTime.Now;

						originalPersonalFolder.credentials.Remove(credential);

						if (user.credentials == null)
						{
							user.credentials = [credential];
						}
						else
						{
							user.credentials.Add(credential);
						}


						_dbContext.Users.Update(user);
						_dbContext.PersonalFolders.Update(originalPersonalFolder);
						_dbContext.Passwords.Update(credential.password);
						_dbContext.Credentials.Update(credential);
						_dbContext.SaveChanges();
						return StatusMessages.UpdateCredential;

					}
					else
					{

						credential.domain = update.domain;
						credential.username = update.username;
						credential.email = update.email;
						credential.remote = update.remote;
						credential.note = update.note;

						originalPersonalFolder.credentials.Remove(credential);

						if (user.credentials == null)
						{
							user.credentials = [credential];
						}
						else
						{
							user.credentials.Add(credential);
						}


						_dbContext.Users.Update(user);
						_dbContext.PersonalFolders.Update(originalPersonalFolder);
						_dbContext.Credentials.Update(credential);
						_dbContext.SaveChanges();
						return StatusMessages.UpdateCredential;
					}
				}
				else
				{
					DomainModel.Credential credential = new DomainModel.Credential();
					DomainModel.PersonalFolder personalFolder = new DomainModel.PersonalFolder();
					DomainModel.PersonalFolder originalPersonalFolder = new DomainModel.PersonalFolder();
					DomainModel.User user = new DomainModel.User();

					try
					{
						user = _dbContext.Users.Include(u => u.folders)
										.Include(u => u.folders).ThenInclude(pf => pf.credentials)
										.Include(u => u.folders).ThenInclude(pf => pf.credentials).ThenInclude(c => c.password)
										.FirstOrDefault(u => u.Id == _userid.ToString());
					}
					catch
					{
						return StatusMessages.UnableToService;
					}

					originalPersonalFolder = user.folders.FirstOrDefault(pf => pf.id == _originalpersonalfolderid);
					personalFolder = user.folders.FirstOrDefault(pf => pf.id == _personalfolderid);
					credential = originalPersonalFolder.credentials.FirstOrDefault(c => c.id == update.id);

					if(user == null)
					{
						return StatusMessages.UnauthorizedAccess;
					}
					if(originalPersonalFolder == null || personalFolder == null)
					{
						return StatusMessages.PersonalFolderNotexist;
					}
					if(credential == null)
					{
						return StatusMessages.CredentialNotexist;
					}

					if (!string.IsNullOrEmpty(update.password) || !string.IsNullOrWhiteSpace(update.password))
					{


						credential.domain = update.domain;
						credential.username = update.username;
						credential.email = update.email;
						credential.remote = update.remote;
						credential.note = update.note;

						SymmetricKey key = _symmetricEncryption.EncryptString(update.password, _configuration);

						credential.password.password = key.password;
						credential.password.aad = key.aad;
						credential.password.updatedate = DateTime.Now;

						personalFolder.credentials.Add(credential);

						originalPersonalFolder.credentials.Remove(credential);

						_dbContext.PersonalFolders.Update(originalPersonalFolder);
						_dbContext.PersonalFolders.Update(personalFolder);
						_dbContext.Passwords.Update(credential.password);
						_dbContext.Credentials.Update(credential);
						_dbContext.SaveChanges();
						return StatusMessages.UpdateCredential;

					}
					else
					{

						credential.domain = update.domain;
						credential.username = update.username;
						credential.email = update.email;
						credential.remote = update.remote;
						credential.note = update.note;

						personalFolder.credentials.Add(credential);

						originalPersonalFolder.credentials.Remove(credential);

						_dbContext.PersonalFolders.Update(originalPersonalFolder);
						_dbContext.PersonalFolders.Update(personalFolder);
						_dbContext.Credentials.Update(credential);
						_dbContext.SaveChanges();
						return StatusMessages.UpdateCredential;
					}
				}
			}
        }

	}
}