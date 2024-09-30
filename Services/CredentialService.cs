using DTOModel.CredentialDTO;
using DTOModel;
using DomainModel;
using DataAccessLayerDB;
using Microsoft.EntityFrameworkCore;
using TransitionObjectMapper;
using Microsoft.AspNetCore.Mvc;
using DataMapper;
using Microsoft.AspNetCore.Identity;
using EncryptionLayer;
using Microsoft.Extensions.Configuration;
using AuthenticationLayer;
using Microsoft.Extensions.Logging;
using EncryptionLayer;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using Mailjet.Client.Resources;

namespace Services
{
    public  class CredentialService
    {
        public List<CredentialDTO> GetCredentialsByClintId(Guid clientid, CredentialDataMapper dataMapper, PSDBContext _dbContext, JwtTokenManager jwtTokenmanager, string jwt)
        {
            if(jwtTokenmanager.GetUserID(jwt, out Guid userid))
            {
				//User splitQuery() to increase performance
				List<TeamCredentialsMap> teamCredentials = _dbContext.Users.Include(u => u.teams)
										   .Include(u => u.teams).ThenInclude(t => t.client)
										   .Include(u => u.teams).ThenInclude(t => t.credentials)
										   .FirstOrDefault(u => u.Id == userid.ToString())
											.teams.Where(t => t.client.id == clientid)
											.Select(t => new TeamCredentialsMap() { teamid = t.id, teamname = t.name, credentials = t.credentials, clientid = clientid }).ToList();
				return dataMapper.ConvertCredentialtoDTO(teamCredentials);
			}
            return new List<CredentialDTO>();
        }

        public List<PersonalCredentialDTO> GetCredentialsByFolderID(Guid folderid, PSDBContext _dbContext, JwtTokenManager jwtTokenmanager, string jwt)
        {
            if(jwtTokenmanager.GetUserID(jwt,out Guid userid))
            {
				if (folderid != Guid.Empty)
				{
					List<PersonalCredentialDTO> personalCredentialList = new List<PersonalCredentialDTO>();
					DomainModel.User user = _dbContext.Users.Include(u => u.folders)
                                                .ThenInclude(pf => pf.credentials)
                                                .FirstOrDefault(u => u.Id == userid.ToString() && u.folders.Any(pf => pf.id == folderid));
					if (user != null)
					{
						foreach (Credential credential in user.folders.Single(pf => pf.id == folderid).credentials)
						{
							personalCredentialList.Add(new PersonalCredentialDTO()
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
					}
					return personalCredentialList;
				}
				else
				{
					List<PersonalCredentialDTO> personalCredentialList = new List<PersonalCredentialDTO>();
					DomainModel.User user = _dbContext.Users.Include(u => u.credentials).FirstOrDefault(u => u.Id == userid.ToString());
					if (user != null)
					{
						foreach (Credential credential in user.credentials)
						{
                            Console.WriteLine(credential.username);
							personalCredentialList.Add(new PersonalCredentialDTO()
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
					}
					return personalCredentialList;
				}
			}
            return new List<PersonalCredentialDTO>();

        }

        public SetStatus AddCredential(PostCredentialDTO postCredential, PSDBContext _dbContext, SymmetricEncryption symmetricEncryption, IConfiguration configuration)
        {
            try
            {
                List<Team> teams = _dbContext.Teams.Include(t => t.credentials)
                                 .Include(t => t.client)
                                 .Where(t => postCredential.teams.Select(ct => ct.teamid).Any(ct => ct == t.id))
                                 .Where(t => postCredential.teams.Select(ct => ct.clientid).Any(ct => ct == t.client.id))
                                 .ToList();

                teams.ForEach(t =>
                {
                    Credential newCredential = new Credential();
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

                _dbContext.SaveChanges();
                return new SetStatus() { status = "OK" };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString(), Console.ForegroundColor = ConsoleColor.Red);
                Console.ForegroundColor = ConsoleColor.Black;
                return new SetStatus() { status = "KO" };
            }

        }
    
        public SetStatus DeleteCredential(DeleteItem deleteCredential, PSDBContext _dbContext, JwtTokenManager jwtTokenmanager, string jwt, ILogger<CredentialService> _logger)
        {
            if(jwtTokenmanager.GetUserID(jwt,out Guid userid))
            {
				try
				{
					Credential credential = _dbContext.Users.Include(u => u.teams)
																.Include(u => u.teams).ThenInclude(t => t.credentials)
																.Include(u => u.teams).ThenInclude(t => t.credentials).ThenInclude(c => c.password)
																.Single(u => u.Id == userid.ToString()).teams
																.Single(t => t.id == deleteCredential.teamid).credentials
																.Single(c => c.id == deleteCredential.id);

                    _dbContext.Passwords.Remove(credential.password);
					_dbContext.Credentials.Remove(credential);
					_dbContext.SaveChanges();
					return new SetStatus() { status = "OK" };
				}
				catch
				{
					return new SetStatus() { status = "KO",errorMessage = "problem connecting to database" };
				}
			}
            _logger.LogError($"Invalid username {userid.ToString()}");
			return new SetStatus() { status = "KO" };

		}

		public SetStatus DeletePersonalCredential(PersonalCredentialId deleteCredential, PSDBContext _dbContext, JwtTokenManager jwtTokenmanager, string jwt, ILogger<CredentialService> _logger)
		{
			if (jwtTokenmanager.GetUserID(jwt, out Guid userid))
			{
				try
				{
                    if(Guid.TryParse(deleteCredential.personalfolderid, out Guid _personalfolderid))
                    {
						Credential credential = _dbContext.Users.Include(u => u.folders)
																	.Include(u => u.folders).ThenInclude(t => t.credentials)
																	.Include(u => u.folders).ThenInclude(t => t.credentials).ThenInclude(c => c.password)
																	.Single(u => u.Id == userid.ToString()).folders
																	.Single(t => t.id == _personalfolderid).credentials
																	.Single(c => c.id == deleteCredential.id);
						_dbContext.Passwords.Remove(credential.password);
						_dbContext.Credentials.Remove(credential);
						_dbContext.SaveChanges();
						return new SetStatus() { status = "OK" };
					}
                    else
                    {
						Credential credential = _dbContext.Users.Include(u => u.credentials)
																.Include(u => u.credentials).ThenInclude(c=> c.password)
                                                                .FirstOrDefault(u => u.Id == userid.ToString()).credentials
                                                                .FirstOrDefault(c => c.id == deleteCredential.id);
						_dbContext.Passwords.Remove(credential.password);
						_dbContext.Credentials.Remove(credential);
						_dbContext.SaveChanges();
						return new SetStatus() { status = "OK" };
					}


				}
				catch
				{
					return new SetStatus() { status = "KO", errorMessage = "problem connecting to database" };
				}
			}
			_logger.LogError($"Invalid username {userid.ToString()}");
			return new SetStatus() { status = "KO" };

		}

		public SetStatus GiveCredential(PostGiveCredentialDTO _giveCredential, PSDBContext _dbContext, JwtTokenManager jwtTokenmanager, [FromServices] IConfiguration _configuration, [FromServices] SymmetricEncryption _symmetricEncryption, string jwt)
        {
            try
            {
                Credential newCredential = new Credential();
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
                return new SetStatus() { status = "OK" };
            }
            catch
            {
                return new SetStatus() { status = "KO" };
            }
        }
    
        public SetStatus Update(PSDBContext _dbContext, PostUpdateCredential update, IConfiguration _configuration, SymmetricEncryption _symmetricEncryption)
        {
            if(string.IsNullOrEmpty(update.password) || string.IsNullOrWhiteSpace(update.password))
            {
				Credential credential = _dbContext.Credentials.Find(update.id);
                if(credential != null)
                {
                    credential.email = update.email;
                    credential.username = update.username;
                    credential.domain = update.domain;
                    credential.note = update.note;
                    credential.remote = update.remote;
                    credential.updatedate = DateTime.Now;

                    _dbContext.Credentials.Update(credential);
                    _dbContext.SaveChanges(true);
                    return new SetStatus() { status = "OK" };
                }
			    return new SetStatus() { status = "KO" };
			}
            else
            {
                Credential credential = _dbContext.Credentials.Include(c => c.password).FirstOrDefault(c => c.id == update.id);
                if(credential != null)
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

                    _dbContext.Passwords.Update(credential.password);
                    _dbContext.Credentials.Update(credential);
                    _dbContext.SaveChanges();

					return new SetStatus() { status = "OK" };
				}
				return new SetStatus() { status = "KO" };
			}
		}
    
        public SetStatus UpdatePersonalCredential(PSDBContext _dbContext, PostUpdatePersonalCredential update, IConfiguration _configuration, SymmetricEncryption _symmetricEncryption, Guid _userid)
        {
            Guid.TryParse(update.personalfolderid, out Guid _personalfolderid);
            Guid.TryParse(update.originalpersonalfolderid, out Guid _originalpersonalfolderid);

            if(_originalpersonalfolderid == Guid.Empty)
            {
                if(_personalfolderid == Guid.Empty) 
                {
                    if(!string.IsNullOrEmpty(update.password) || !string.IsNullOrWhiteSpace(update.password))
                    {
						Credential credential = new Credential();
						DomainModel.User user = new DomainModel.User();

						user = _dbContext.Users.Include(u => u.credentials).FirstOrDefault(u => u.Id == _userid.ToString());
						credential = _dbContext.Credentials.Include(c => c.password).FirstOrDefault(c => c.id == update.id);

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

						_dbContext.Users.Update(user);
                        _dbContext.Passwords.Update(credential.password);
                        _dbContext.Credentials.Update(credential);
                        _dbContext.SaveChanges();
						return new SetStatus() { status = "OK" };

					}
                    else
                    {
						Credential credential = new Credential();
						DomainModel.User user = new DomainModel.User();

						user = _dbContext.Users.Include(u => u.credentials).FirstOrDefault(u => u.Id == _userid.ToString());
						credential = _dbContext.Credentials.Include(c => c.password).FirstOrDefault(c => c.id == update.id);

						credential.domain = update.domain;
						credential.username = update.username;
						credential.email = update.email;
						credential.remote = update.remote;
						credential.note = update.note;

						user.credentials.Remove(credential);

						_dbContext.Users.Update(user);
						_dbContext.Credentials.Update(credential);
						_dbContext.SaveChanges();
						return new SetStatus() { status = "OK" };
					}
                }
                else
                {
					if (!string.IsNullOrEmpty(update.password) || !string.IsNullOrWhiteSpace(update.password))
					{
						Credential credential = new Credential();
                        PersonalFolder personalFolder = new PersonalFolder();
						DomainModel.User user = new DomainModel.User();

						user = _dbContext.Users.Include(u => u.credentials).FirstOrDefault(u => u.Id == _userid.ToString());
						personalFolder = _dbContext.PersonalFolders.Include(pf => pf.credentials).FirstOrDefault(pf => pf.id ==_personalfolderid);
						credential = _dbContext.Credentials.Include(c => c.password).FirstOrDefault(c => c.id == update.id);

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

						user.credentials.Remove(credential);

						_dbContext.Users.Update(user);
						_dbContext.PersonalFolders.Update(personalFolder);
						_dbContext.Passwords.Update(credential.password);
						_dbContext.Credentials.Update(credential);
						_dbContext.SaveChanges();
						return new SetStatus() { status = "OK" };

					}
					else
					{
						Credential credential = new Credential();
						PersonalFolder personalFolder = new PersonalFolder();
						DomainModel.User user = new DomainModel.User();

						user = _dbContext.Users.Include(u => u.credentials).FirstOrDefault(u => u.Id == _userid.ToString());
						personalFolder = _dbContext.PersonalFolders.Include(pf => pf.credentials).FirstOrDefault(pf => pf.id == _personalfolderid);
						credential = _dbContext.Credentials.Include(c => c.password).FirstOrDefault(c => c.id == update.id);

						credential.domain = update.domain;
						credential.username = update.username;
						credential.email = update.email;
						credential.remote = update.remote;
						credential.note = update.note;

						personalFolder.credentials.Add(credential);
						user.credentials.Remove(credential);

						_dbContext.Users.Update(user);
						_dbContext.PersonalFolders.Update(personalFolder);
						_dbContext.Credentials.Update(credential);
						_dbContext.SaveChanges();
						return new SetStatus() { status = "OK" };
					}
				}
            }
            else
            {
				if (_personalfolderid == Guid.Empty)
				{
					if (!string.IsNullOrEmpty(update.password) || !string.IsNullOrWhiteSpace(update.password))
					{
						Credential credential = new Credential();
						PersonalFolder originalPersonalFolder = new PersonalFolder();
						DomainModel.User user = new DomainModel.User();

						user = _dbContext.Users.Include(u => u.credentials).FirstOrDefault(u => u.Id == _userid.ToString());
						originalPersonalFolder = _dbContext.PersonalFolders.Include(pf => pf.credentials).FirstOrDefault(pf => pf.id == _originalpersonalfolderid);

						credential = _dbContext.Credentials.Include(c => c.password).FirstOrDefault(c => c.id == update.id);

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
						user.credentials.Add(credential);

						_dbContext.Users.Update(user);
						_dbContext.PersonalFolders.Update(originalPersonalFolder);
						_dbContext.Passwords.Update(credential.password);
						_dbContext.Credentials.Update(credential);
						_dbContext.SaveChanges();
						return new SetStatus() { status = "OK" };

					}
					else
					{
						Credential credential = new Credential();
						PersonalFolder originalPersonalFolder = new PersonalFolder();
						DomainModel.User user = new DomainModel.User();

						user = _dbContext.Users.Include(u => u.credentials).FirstOrDefault(u => u.Id == _userid.ToString());
						originalPersonalFolder = _dbContext.PersonalFolders.Include(pf => pf.credentials).FirstOrDefault(pf => pf.id == _originalpersonalfolderid);
						credential = _dbContext.Credentials.Include(c => c.password).FirstOrDefault(c => c.id == update.id);

						credential.domain = update.domain;
						credential.username = update.username;
						credential.email = update.email;
						credential.remote = update.remote;
						credential.note = update.note;

						originalPersonalFolder.credentials.Remove(credential);
						user.credentials.Add(credential);

						_dbContext.Users.Update(user);
						_dbContext.PersonalFolders.Update(originalPersonalFolder);
						_dbContext.Credentials.Update(credential);
						_dbContext.SaveChanges();
						return new SetStatus() { status = "OK" };
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(update.password) || !string.IsNullOrWhiteSpace(update.password))
					{
						Credential credential = new Credential();
						PersonalFolder personalFolder = new PersonalFolder();
						PersonalFolder originalPersonalFolder = new PersonalFolder();

						originalPersonalFolder = _dbContext.PersonalFolders.Include(pf => pf.credentials).FirstOrDefault(pf => pf.id == _originalpersonalfolderid);
						personalFolder = _dbContext.PersonalFolders.Include(pf => pf.credentials).FirstOrDefault(pf => pf.id == _personalfolderid);
						credential = _dbContext.Credentials.Include(c => c.password).FirstOrDefault(c => c.id == update.id);

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
						return new SetStatus() { status = "OK" };

					}
					else
					{
						Credential credential = new Credential();
						PersonalFolder personalFolder = new PersonalFolder();
						PersonalFolder originalPersonalFolder = new PersonalFolder();

						originalPersonalFolder = _dbContext.PersonalFolders.Include(pf => pf.credentials).FirstOrDefault(pf => pf.id == _originalpersonalfolderid);
						personalFolder = _dbContext.PersonalFolders.Include(pf => pf.credentials).FirstOrDefault(pf => pf.id == _personalfolderid);
						credential = _dbContext.Credentials.Include(c => c.password).FirstOrDefault(c => c.id == update.id);

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
						return new SetStatus() { status = "OK" };
					}
				}
			}
        }

	}
}
