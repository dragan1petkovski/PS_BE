using DataAccessLayerDB;
using DomainModel;
using DTOModel;
using DTOModel.UserDTO;
using Microsoft.AspNetCore.Identity;
using EmailService;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Microsoft.EntityFrameworkCore;
using AuthenticationLayer;
using DataMapper;
using DTOModel.TeamDTO;
using Microsoft.Extensions.Logging;
using LogginMessages;
using Microsoft.AspNetCore.Mvc;

namespace Services
{
    public class UserService
    {
        public List<User> GetAllUsers(PSDBContext _dbContext)
        {

            return _dbContext.Users.ToList();
        }

        public async Task<SetStatus> AddUser(PostUserDTO _newUser, PSDBContext _dbContext, IConfiguration _configuration, MailJetMailer _smtpclient, ILogger<UserService> _logger)
        {
            try
            {
                if(_dbContext.Users.Any(u => u.Email == _newUser.email && u.NormalizedEmail == _newUser.email.Normalize()))
                {
                    _logger.LogError($"{DateTime.Now} - User exist with the email: {_newUser.email}");
					return new SetStatus() { status = "KO", errorMessage = $"User already exist with e-mail {_newUser.email}" };
				}
                else
                {
					User newUser = new User();
					PasswordHasher<User> passwordHasher = new PasswordHasher<User>();
					newUser.createdate = DateTime.Now;
					newUser.Id = Guid.NewGuid().ToString();
					newUser.firstname = _newUser.firstname;
					newUser.lastname = _newUser.lastname;
					newUser.Email = _newUser.email;
					newUser.NormalizedEmail = newUser.Email.Normalize();
					newUser.UserName = _newUser.username;
					newUser.teams = _dbContext.Teams.Where(t => _newUser.clientTeamPairs.Select(ct => ct.teamid).ToList().Any(ct => ct == t.id)).Distinct().ToList();
					newUser.EmailConfirmed = false;
					newUser.updatedate = DateTime.Now;

					string roleID = _dbContext.Roles.FirstOrDefault(r => r.Name == "User").Id;
					_dbContext.UserRoles.Add(new IdentityUserRole<string>() { RoleId = roleID, UserId = newUser.Id });


					EmailNotification setnewpassword = new EmailNotification();
					setnewpassword.Id = Guid.NewGuid();
					setnewpassword.type = TypeEnum.SetNewPassword;
					setnewpassword.action = ActionEnum.Pending;
					setnewpassword.isClicked = false;
					setnewpassword.createdon = DateTime.Now;
					setnewpassword.user = newUser;

					string body = _smtpclient.SetNewPasswordBody(newUser.NormalizedUserName, setnewpassword.Id.ToString());

					if (await _smtpclient.SendMailMessage(_configuration, newUser.NormalizedEmail, body, _smtpclient.Subject))
					{
                        try
                        {
							_dbContext.EmailNotifiers.Add(setnewpassword);
							_dbContext.Users.Add(newUser);
							_dbContext.SaveChanges();
							return new SetStatus() { status = "OK" };
						}
                        catch (Exception ex)
                        {
                            _logger.LogError(DatabaseLog.DBConnectionLog(ex.ToString()));
							return new SetStatus() { status = "KO", errorMessage=DatabaseLog.DBConnectionMsg };
						}

					}
					else
					{
						return new SetStatus() { status = "KO", errorMessage=$"Cannot send mail to the user, check if the mail is correct or try again later" };
					}

				}

			}
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now} - User can not be added at the moment - Details\n {ex.ToString()}");
                return new SetStatus() { status = "KO", errorMessage= $"User cannot be added at the moment" };
                
            }
        }

        public async Task<bool> ChangePassword(PSDBContext _dbContext, UserManager<User> _userManager, ChangePasswordDTO changePassword, JwtTokenManager _jwtTokenManager, string jwt, ILogger<UserService> _logger)
		{
            if(_jwtTokenManager.GetUserID(jwt, out Guid userid))
            {
				User loggedInUser = new User(); 
                try
                {
					loggedInUser = _dbContext.Users.Find(userid.ToString());
				}
                catch(Exception ex)
                {
                    _logger.LogError(DatabaseLog.DBConnectionLog(ex.ToString()));
                    return false;
                }
				if (loggedInUser != null)
				{
					if (changePassword.confirmPassword == changePassword.newPassword)
					{
                        EmailNotification changePasswordRequest = new EmailNotification();
                        try
                        {
							changePasswordRequest = _dbContext.EmailNotifiers.FirstOrDefault(en => en.validationCode == changePassword.verificationcode && en.user.Id == loggedInUser.Id);
						}
                        catch(Exception ex)
                        {
							_logger.LogError(DatabaseLog.DBConnectionLog(ex.ToString()));
							return false;
						}
                        if (changePasswordRequest.isClicked && changePasswordRequest.action != ActionEnum.Pending  && changePasswordRequest.type != TypeEnum.ChangePassword && (DateTime.Now - changePasswordRequest.createdon) > new TimeSpan(0, 0, 59))
                        {
                            return false;
                        }
                        else
                        {
                            try
                            {
								IdentityResult result = await _userManager.ChangePasswordAsync(loggedInUser, changePassword.oldPassword, changePassword.newPassword);
								if (result.Succeeded)
								{
									_dbContext.EmailNotifiers.Remove(changePasswordRequest);
									_dbContext.SaveChanges();
									return true;
								}
								else
								{
									return false;
								}
							}
                            catch (Exception ex)
                            {
								_logger.LogError(DatabaseLog.DBConnectionLog(ex.ToString()));
								return false;
                            }	
						}
					}
					return false;
				}
				return false;
			}
			return false;
		}
    
        public async Task<bool> GetVerificationCode(PSDBContext _dbContext, IConfiguration _configuration, MailJetMailer smtpClinet ,JwtTokenManager _jwtTokenManager, string jwt, ILogger<UserService> _logger)
        {
            if(_jwtTokenManager.GetUserID(jwt,out Guid _userid))
            {
                User loggedUser = new User();
                try
                {
					loggedUser = _dbContext.Users.Find(_userid.ToString());
				}
                catch(Exception ex)
                {
					_logger.LogError(DatabaseLog.DBConnectionLog(ex.ToString()));
					return false;
				}
                if(loggedUser != null)
                {
                    EmailNotification verificationCode = new EmailNotification();
                    Random random = new Random();
                    Guid verificationCodeid = Guid.NewGuid();
					int code = random.Next(10000000, 100000000);
                    
                    verificationCode.user = loggedUser;
					verificationCode.action = ActionEnum.Pending;
                    verificationCode.isClicked = false;
                    verificationCode.validationCode = code;
                    verificationCode.Id = verificationCodeid;
					verificationCode.createdon = DateTime.Now;

                    try
                    {
						if (await smtpClinet.SendMailMessage(_configuration, loggedUser.NormalizedEmail, smtpClinet.GetVerificationCode(loggedUser.NormalizedUserName, code), smtpClinet.Subject))
						{
							_dbContext.EmailNotifiers.Add(verificationCode);
							_dbContext.SaveChanges();
							return true;
						}
						else
						{
							return false;
						}
					}
                    catch(Exception ex)
                    {
						_logger.LogError($"{DateTime.Now} - Database issue or connection issue to mail server -\n {ex.ToString()}\n");
						return false;
					}
                    
				}
                return false;
            }
            return false;
        }
    
        public SetStatus Update(PostUpdateUser update, PSDBContext _dbContext, ILogger<UserService> _logger)
        {
            User updateUser = new User();
            try
            {
				updateUser = _dbContext.Users.Include(u => u.teams).FirstOrDefault(u => u.Id == update.id.ToString());
			}
            catch(Exception ex)
            {
				_logger.LogError(DatabaseLog.DBConnectionLog(ex.ToString()));
				return new SetStatus() { status = "KO", errorMessage = DatabaseLog.DBConnectionMsg };
			}
            if(updateUser != null)
            {
                updateUser.UserName = update.username;
                updateUser.Email = update.email;
                updateUser.firstname = update.firstname;
                updateUser.lastname = update.lastname;
                updateUser.NormalizedEmail = updateUser.Email.Normalize();
                updateUser.NormalizedUserName = updateUser.UserName.Normalize();
                updateUser.teams.RemoveAll(t => true);
                

                foreach(ClientTeamPair ct in update.clientTeamPairs)
                {
                    Team team = new Team();
                    try
                    {
                        team = _dbContext.Teams.Include(t => t.client).FirstOrDefault(t => t.id == ct.teamid && t.client.id == ct.clientid);
                    }
					catch (Exception ex)
					{
						_logger.LogError(DatabaseLog.DBConnectionLog(ex.ToString()));
						return new SetStatus() { status = "KO", errorMessage=DatabaseLog.DBConnectionMsg };
					}
					if (team != null)
                    {
                        updateUser.teams.Add(team);
                    }
                }
				updateUser.updatedate = DateTime.Now;
				_dbContext.Users.Update(updateUser);
                _dbContext.SaveChanges();
				return new SetStatus() { status = "OK" };
			}
            return new SetStatus() { status = "KO" };
        }

        public UpdateUserDTO GetUserDetails(Guid Id, PSDBContext _dbContext, JwtTokenManager _jwtTokenManager, string jwt, [FromServices] ILogger<UserService> _logger)
        {
            if(_jwtTokenManager.GetUserID(jwt, out Guid _userid))
            {
                TeamDataMapper teamService = new TeamDataMapper();
                User loggedUser = new User();
                try
                {
                    loggedUser =_dbContext.Users.Include(u => u.teams)
                                                .Include(u => u.teams).ThenInclude(t => t.client)
                                                .FirstOrDefault(u => u.Id == Id.ToString());
                }
				catch(Exception ex)
                {
                    _logger.LogError(DatabaseLog.DBConnectionLog(ex.ToString()));
                    return null;
                }
				if (loggedUser != null)
                {
					UpdateUserDTO output = new UpdateUserDTO();
                    output.id = Guid.Parse(loggedUser.Id);
					output.firstname = loggedUser.firstname;
                    output.lastname = loggedUser.lastname;
                    output.username = loggedUser.UserName;
                    output.email = loggedUser.Email;

                    output.clientTeamMapping = teamService.ConvertToClientTeamMappingList(loggedUser.teams);
					return output;
				}
                return null;
			}
            return null;
        }

        public SetStatus Delete(Guid Id, PSDBContext _dbContext, ILogger<UserService> _logger)
        {
            User user = new User();
            List<EmailNotification> notifications = new List<EmailNotification>(); 
			try
            {
                user = _dbContext.Users.Include(u => u.credentials)
                                        .Include(u => u.credentials).ThenInclude(c => c.password)
                                        .Include(u => u.teams)
                                        .Include(u => u.folders)
                                        .Include(u => u.folders).ThenInclude(pf => pf.credentials)
                                        .Include(u => u.folders).ThenInclude(ph => ph.credentials).ThenInclude(c => c.password)
                                        .FirstOrDefault(u => u.Id == Id.ToString());
                notifications = _dbContext.EmailNotifiers.Include(en => en.user).Where(en => en.user.Id == user.Id).ToList();
			}
            catch (Exception ex)
            {
				_logger.LogError(DatabaseLog.DBConnectionLog(ex.ToString()));
				return new SetStatus() { status = "KO", errorMessage= DatabaseLog.DBConnectionMsg };
			}
            if(notifications != null) 
            {
                if(notifications.Count > 0)
                {
                    try
                    {
						_dbContext.EmailNotifiers.RemoveRange(notifications);
					}
                    catch(Exception ex)
                    {
						_logger.LogError(DatabaseLog.DBConnectionLog(ex.ToString()));
					}
                    
                }
            }
            if (user != null)
            {
                List<Password> passwordsForRemove = user.credentials.Select(c => c.password).ToList();
                user.folders.ForEach(pf => passwordsForRemove.AddRange(pf.credentials.Select(pf => pf.password).ToList()));

                List<Credential> credentialsForRemove = user.credentials;
                user.folders.ForEach(pf => credentialsForRemove.AddRange(pf.credentials));


                List<PersonalFolder> personalFoldersForRemove = user.folders;

                try
                {
					_dbContext.Passwords.RemoveRange(passwordsForRemove);
					_dbContext.Credentials.RemoveRange(credentialsForRemove);
					_dbContext.PersonalFolders.RemoveRange(personalFoldersForRemove);
					user.teams.RemoveAll(t => true);

					_dbContext.Users.Remove(user);

					_dbContext.SaveChanges();
					return new SetStatus() { status = "OK" };
				}
                catch (Exception ex)
                {
					_logger.LogError(DatabaseLog.DBConnectionLog(ex.ToString()));
					return new SetStatus() { status = "KO",errorMessage=DatabaseLog.DBConnectionMsg }; 
				}
            }
            return new SetStatus() { status = "KO", errorMessage = DatabaseLog.DBConnectionMsg };
        }
    }
}
