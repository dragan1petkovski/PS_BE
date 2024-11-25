using DataAccessLayerDB;
using DomainModel;
using DTO.Team;
using DTO.User;
using Microsoft.AspNetCore.Identity;
using LogginMessages;
using Microsoft.Extensions.Configuration;
using EmailService;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Logging;


using DataMapper;

namespace AppServices
{
    public class UserService
    {
        public List<DomainModel.User> GetAllUsers(PSDBContext _dbContext)
        {

            return _dbContext.Users.ToList();
        }

		public async Task<(StatusMessages,DTO.User.User)> AddUser(string type , PostUser _newUser, PSDBContext _dbContext, IConfiguration _configuration, MailJetMailer _smtpclient, ILogger<UserService> _logger)
        {
			bool ifExit = true;
			DTO.User.User syncUser = new DTO.User.User();
			try
			{
				ifExit = _dbContext.Users.Any(u => u.Email == _newUser.email || u.NormalizedEmail == _newUser.email.Normalize() || u.UserName == _newUser.username || u.NormalizedUserName == _newUser.username.Normalize());
			}
			catch
			{
				Console.WriteLine("First unable to Server");
				return (StatusMessages.UnableToService,null);
			}

			if (ifExit)
			{
				_logger.LogError($"{DateTime.Now} - User exist with\nEmail: {_newUser.email} or\nUsername: {_newUser.username}");
				return (StatusMessages.UserExist,null);
			}
			else
			{
				DomainModel.User newUser = new DomainModel.User();
				PasswordHasher<DomainModel.User> passwordHasher = new PasswordHasher<DomainModel.User>();
				newUser.createdate = DateTime.Now;
				newUser.Id = Guid.NewGuid().ToString();
				newUser.firstname = _newUser.firstname;
				newUser.lastname = _newUser.lastname;
				newUser.Email = _newUser.email;
				newUser.NormalizedEmail = newUser.Email.Normalize();
				newUser.UserName = _newUser.username;
				newUser.NormalizedUserName = _newUser.username.Normalize();
				if(_newUser.clientTeamPairs != null)
				{
					newUser.teams = _dbContext.Teams.Where(t => _newUser.clientTeamPairs.Select(ct => ct.teamid).ToList().Any(ct => ct == t.id)).Distinct().ToList();
				}
				newUser.EmailConfirmed = false;
				newUser.updatedate = DateTime.Now;
				string rolename = null;
				if(type == "user")
				{
					IdentityRole role = _dbContext.Roles.FirstOrDefault(r => r.Name == "User");
					rolename = role.Name;
					_dbContext.UserRoles.Add(new IdentityUserRole<string>() { RoleId = role.Id, UserId = newUser.Id });
				}
				else if(type == "admin" && _newUser.clientTeamPairs == null)
				{
					IdentityRole role = _dbContext.Roles.FirstOrDefault(r => r.Name == "Administrator");
					rolename = role.Name;
					_dbContext.UserRoles.Add(new IdentityUserRole<string>() { RoleId = role.Id, UserId = newUser.Id });
				}
				else
				{
					return (StatusMessages.InvalidRequest, null);
				}
				EmailNotification setnewpassword = new EmailNotification();
				setnewpassword.Id = Guid.NewGuid();
				setnewpassword.type = TypeEnum.SetNewPassword;
				setnewpassword.action = ActionEnum.Pending;
				setnewpassword.isClicked = false;
				setnewpassword.createdon = DateTime.Now;
				setnewpassword.user = newUser;


				syncUser.username = newUser.UserName;
				syncUser.email = newUser.Email;
				syncUser.firstname = newUser.firstname;
				syncUser.lastname = newUser.lastname;
				syncUser.id = Guid.Parse(newUser.Id);
				syncUser.createdate = newUser.createdate;
				syncUser.updatedate = newUser.updatedate;
				syncUser.rolename = rolename;


				string body = _smtpclient.ResetPasswordBody(newUser.NormalizedUserName, setnewpassword.Id.ToString());

				try
				{
					if (await _smtpclient.SendMailMessage(_configuration, newUser.NormalizedEmail, body, _smtpclient.Subject))
					{
						_dbContext.Users.Add(newUser);
					}
					else
					{
						_logger.LogError($"{DateTime.Now} - Server cannot connecto to the mailjet server to send the mail");
						return (StatusMessages.UnableToService, null);
					}
					_dbContext.EmailNotifiers.Add(setnewpassword);
					
					_dbContext.SaveChanges();
					return (StatusMessages.AddNewUser,syncUser);
				}
				catch (Exception ex)
				{
					_logger.LogError(DatabaseLog.DBConnectionLog(ex.ToString()));
					return (StatusMessages.UnableToService,null);
				}


			}
		}

        public async Task<StatusMessages> ResetPassword(Guid userid, PSDBContext _dbContext, UserManager<DomainModel.User> _userManager, MailJetMailer _smtpclient, IConfiguration _configuration, ILogger<UserService> _logger)
		{
			DomainModel.User user = null;
			try
			{
				user = _dbContext.Users.Find(userid.ToString());
			}
			catch(Exception ex)
			{
				_logger.LogCritical($"{DateTime.Now} - Database is not accessible \n{ex.Message}\n\n");
				return StatusMessages.UnableToService;
			}
			if(user == null)
			{
				return StatusMessages.UserNotexist;
			}

			EmailNotification resetPassword = new EmailNotification();
			resetPassword.Id = Guid.NewGuid();
			resetPassword.type = TypeEnum.ResetPassword;
			resetPassword.action = ActionEnum.Pending;
			resetPassword.isClicked = false;
			resetPassword.createdon = DateTime.Now;
			resetPassword.user = user;
			resetPassword.validationCode = await _userManager.GeneratePasswordResetTokenAsync(user);

			string body = _smtpclient.SetNewPasswordBody(user.NormalizedUserName, resetPassword.Id.ToString());

			if (await _smtpclient.SendMailMessage(_configuration, user.NormalizedEmail, body, _smtpclient.Subject))
			{
				try
				{
					_dbContext.EmailNotifiers.Add(resetPassword);
					_dbContext.SaveChanges();
					return StatusMessages.ResetPassword;
				}
				catch (Exception ex)
				{
					_logger.LogError(DatabaseLog.DBConnectionLog(ex.ToString()));
					return StatusMessages.UnableToService;
				}

			}
			else
			{
				_logger.LogError($"{DateTime.Now} - Server cannot connecto to the mailjet server to send the mail");
				return StatusMessages.UnableToService;
			}

		}

		public async Task<StatusMessages> ChangePassword(Guid userid, PSDBContext _dbContext, UserManager<DomainModel.User> _userManager, ChangePassword changePassword, ILogger<UserService> _logger)
		{
			DomainModel.User loggedInUser = null; 
            try
            {
				loggedInUser = _dbContext.Users.Find(userid.ToString());
			}
            catch(Exception ex)
            {
				_logger.LogError(DatabaseLog.DBConnectionLog(ex.ToString()));
				return StatusMessages.UnableToService;
			}
			if (loggedInUser == null)
			{
				return StatusMessages.UnauthorizedAccess;
			}

			if (changePassword.confirmPassword != changePassword.newPassword)
			{
				return StatusMessages.IncorrectConfirmPassword;
			}
			EmailNotification changePasswordRequest = new EmailNotification();
			try
			{
				changePasswordRequest = _dbContext.EmailNotifiers.FirstOrDefault(en => en.validationCode == changePassword.verificationcode.ToString() && en.user.Id == loggedInUser.Id);
			}
			catch (Exception ex)
			{
				_logger.LogError(DatabaseLog.DBConnectionLog(ex.ToString()));
				return StatusMessages.UnableToService;
			}
			if (changePasswordRequest.isClicked && changePasswordRequest.action != ActionEnum.Pending && changePasswordRequest.type != TypeEnum.ChangePassword && (DateTime.Now - changePasswordRequest.createdon) > new TimeSpan(0, 0, 59))
			{
				return StatusMessages.InvalidVerificationCode;
			}
			try
			{
				IdentityResult result = await _userManager.ChangePasswordAsync(loggedInUser, changePassword.oldPassword, changePassword.newPassword);
				if (result.Succeeded)
				{
					_dbContext.EmailNotifiers.Remove(changePasswordRequest);
					_dbContext.SaveChanges();
					return StatusMessages.PasswordChange;
				}
				else
				{
					return StatusMessages.IncorrectPasswordComplexity;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(DatabaseLog.DBConnectionLog(ex.ToString()));
				return StatusMessages.UnableToService;
			}
		}
    
        public async Task<StatusMessages> GetVerificationCode(Guid _userid, PSDBContext _dbContext, IConfiguration _configuration, MailJetMailer smtpClinet , ILogger<UserService> _logger)
        {
			DomainModel.User loggedUser = new DomainModel.User();
			try
			{
				loggedUser = _dbContext.Users.Find(_userid.ToString());
			}
			catch (Exception ex)
			{
				_logger.LogError(DatabaseLog.DBConnectionLog(ex.ToString()));
				return StatusMessages.UnableToService;
			}
			if (loggedUser == null)
			{
				return StatusMessages.UnauthorizedAccess;
			}


			EmailNotification verificationCode = new EmailNotification();
			Random random = new Random();
			Guid verificationCodeid = Guid.NewGuid();
			int code = random.Next(10000000, 100000000);

			verificationCode.user = loggedUser;
			verificationCode.action = ActionEnum.Pending;
			verificationCode.isClicked = false;
			verificationCode.validationCode = code.ToString();
			verificationCode.Id = verificationCodeid;
			verificationCode.createdon = DateTime.Now;

			try
			{
				if (await smtpClinet.SendMailMessage(_configuration, loggedUser.NormalizedEmail, smtpClinet.GetVerificationCode(loggedUser.NormalizedUserName, code), smtpClinet.Subject))
				{
					_dbContext.EmailNotifiers.Add(verificationCode);
					_dbContext.SaveChanges();
					return StatusMessages.SendVerificationCode;
				}
				return StatusMessages.UnableToService;
			}
			catch (Exception ex)
			{
				_logger.LogError($"{DateTime.Now} - Database issue or connection issue to mail server -\n {ex.ToString()}\n");
				return StatusMessages.UnableToService;
			}
		}
    
        public (StatusMessages,DTO.User.User) Update(PostUpdateUser update, PSDBContext _dbContext, ILogger<UserService> _logger)
        {
			DomainModel.User updateUser = new DomainModel.User();
			DTO.User.User syncUser = new DTO.User.User();
            try
            {
				updateUser = _dbContext.Users.Include(u => u.teams).FirstOrDefault(u => u.Id == update.id.ToString());
			}
            catch(Exception ex)
            {
				_logger.LogError(DatabaseLog.DBConnectionLog(ex.ToString()));
				return (StatusMessages.UnableToService,null);
			}
            if(updateUser == null)
            {
				return (StatusMessages.UserNotexist,null);
			}

			updateUser.UserName = update.username;
			updateUser.Email = update.email;
			updateUser.firstname = update.firstname;
			updateUser.lastname = update.lastname;
			updateUser.NormalizedEmail = updateUser.Email.Normalize();
			updateUser.NormalizedUserName = updateUser.UserName.Normalize();
			updateUser.teams.RemoveAll(t => true);
			
			syncUser.username = updateUser.UserName;
			syncUser.email = updateUser.Email;
			syncUser.firstname = updateUser.firstname;
			syncUser.lastname = updateUser.lastname;
			syncUser.id = Guid.Parse(updateUser.Id);
			syncUser.createdate = updateUser.createdate;


			foreach (ClientTeamPair ct in update.clientTeamPairs)
			{
				DomainModel.Team team = new DomainModel.Team();
				try
				{
					team = _dbContext.Teams.Include(t => t.client).FirstOrDefault(t => t.id == ct.teamid && t.client.id == ct.clientid);
				}
				catch (Exception ex)
				{
					_logger.LogError(DatabaseLog.DBConnectionLog(ex.ToString()));
					return (StatusMessages.UnableToService,null);
				}
				if (team != null)
				{
					updateUser.teams.Add(team);
				}
			}
			try
			{
				updateUser.updatedate = DateTime.Now;
				_dbContext.Users.Update(updateUser);
				_dbContext.SaveChanges();
				syncUser.updatedate = updateUser.updatedate;
				return (StatusMessages.UpdateUser, syncUser);
			}
			catch
			{
				return (StatusMessages.UnableToService, null);
			}

		}

        public UpdateUser GetUserDetails(Guid Id, PSDBContext _dbContext, ILogger _logger)
        {
			TeamDataMapper teamService = new TeamDataMapper();
			DomainModel.User loggedUser = new DomainModel.User();
			try
			{
				loggedUser = _dbContext.Users.Include(u => u.teams)
											.Include(u => u.teams).ThenInclude(t => t.client)
											.FirstOrDefault(u => u.Id == Id.ToString());
			}
			catch (Exception ex)
			{
				_logger.LogError(DatabaseLog.DBConnectionLog(ex.ToString()));
				return null;
			}
			if (loggedUser == null)
			{
				return null;
			}

			UpdateUser output = new UpdateUser();
			output.id = Guid.Parse(loggedUser.Id);
			output.firstname = loggedUser.firstname;
			output.lastname = loggedUser.lastname;
			output.username = loggedUser.UserName;
			output.email = loggedUser.Email;

			output.clientTeamMapping = teamService.ConvertToClientTeamMappingList(loggedUser.teams);
			return output;
		}

        public StatusMessages Delete(Guid Id, PSDBContext _dbContext, ILogger<UserService> _logger)
        {
			DomainModel.User user = null;
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
				return StatusMessages.UnableToService;
			}
			if (user == null)
			{
				return StatusMessages.UnauthorizedAccess;
			}

			if (notifications == null) 
            {
				return StatusMessages.InvalidVerificationCode;
            }
			if (notifications.Count > 0)
			{
				try
				{
					_dbContext.EmailNotifiers.RemoveRange(notifications);
				}
				catch (Exception ex)
				{
					_logger.LogError(DatabaseLog.DBConnectionLog(ex.ToString()));
					return StatusMessages.UnableToService;
				}

			}

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
				return StatusMessages.DeleteUser;
			}
			catch (Exception ex)
			{
				_logger.LogError(DatabaseLog.DBConnectionLog(ex.ToString()));
				return StatusMessages.UnableToService;
			}
		}
    }
}
