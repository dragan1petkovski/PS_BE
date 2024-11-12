using DataAccessLayerDB;
using DataMapper;
using DomainModel;
using DTO.Password;
using EncryptionLayer;
using LogginMessages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TransitionObjectMapper;

namespace AppServices
{
    public class PasswordService
    {

		public (StatusMessages, DTO.Password.Password) GetCredentialPasswordById(Guid _userid, PSDBContext _dbContext, SymmetricEncryption symmetricEncryption, IConfiguration configuration, PasswordDataMapper dataMapper, Guid id, Guid parentid)
		{
			User LoggedUser = null; 
			try
			{
				LoggedUser = _dbContext.Users.Include(u => u.teams).AsSplitQuery()
								  .Include(u => u.teams).ThenInclude(t => t.credentials).AsSplitQuery()
								  .Include(u => u.teams).ThenInclude(t => t.credentials).ThenInclude(c => c.password).AsSplitQuery()
								  .FirstOrDefault(u => u.Id == _userid.ToString());
			}
			catch (Exception ex)
			{
				return (StatusMessages.UnableToService,null);
			}

			if (LoggedUser == null)
			{
				return (StatusMessages.UnauthorizedAccess, null);
			}

			Team team = LoggedUser.teams.FirstOrDefault(c => c.id == parentid);
			if (team == null)
			{
				return (StatusMessages.TeamNotexist, null);
			}

			Credential credential = team.credentials.FirstOrDefault(cr => cr.id == id);
			if (credential == null)
			{
				return (StatusMessages.CredentialNotexist, null);
			}

			if (credential.password == null)
			{
				//Critical Log
				return (StatusMessages.CredentialNotexist, null);
			}

			SymmetricKey key = dataMapper.ConvertPasswordToSymmetricKey(credential.password);
			string decryptedpassword = symmetricEncryption.DecryptString(key.password, key.aad, configuration);
			return (StatusMessages.Ok, new DTO.Password.Password() { password = decryptedpassword });


		}

		public (StatusMessages, DTO.Password.Password) GetCertificatePasswordById(Guid _userid, PSDBContext _dbContext, SymmetricEncryption symmetricEncryption, IConfiguration configuration, PasswordDataMapper dataMapper, Guid id, Guid parentid)
		{
			User LoggedUser = null;
			try
			{
				LoggedUser = _dbContext.Users.Include(u => u.teams)
												  .Include(u => u.teams).ThenInclude(t => t.client)
												  .Include(u => u.teams).ThenInclude(t => t.certificates)
												  .Include(u => u.teams).ThenInclude(t => t.certificates).ThenInclude(c => c.password)
												  .FirstOrDefault(u => u.Id == _userid.ToString());
			}
			catch (Exception ex)
			{
				return (StatusMessages.UnableToService,null);
			}

			if (LoggedUser == null)
			{
				return (StatusMessages.UnauthorizedAccess,null);
			}

			Team team = LoggedUser.teams.FirstOrDefault(c => c.id == parentid);
			if (team == null)
			{
				return (StatusMessages.TeamNotexist, null);
			}

			Certificate credential = team.certificates.FirstOrDefault(cr => cr.id == id);
			if (credential == null)
			{
				return (StatusMessages.CredentialNotexist, null);
			}

			if (credential.password == null)
			{
				return (StatusMessages.CredentialNotexist, null);
			}

			SymmetricKey key = dataMapper.ConvertPasswordToSymmetricKey(credential.password);
			string decryptedpassword = symmetricEncryption.DecryptString(key.password, key.aad, configuration);
			return (StatusMessages.Ok, new DTO.Password.Password() { password = decryptedpassword });


		}

		public (StatusMessages, DTO.Password.Password) GetPersonalPasswordById(Guid _userid, PSDBContext _dbContext, SymmetricEncryption symmetricEncryption, IConfiguration configuration, PasswordDataMapper dataMapper, Guid id, Guid parentid)
		{
			User user = null;
			try
			{
				user = _dbContext.Users.Include(u => u.credentials)
											.Include(u => u.credentials).ThenInclude(c => c.password).FirstOrDefault(u => u.Id ==_userid.ToString());
			}
			catch
			{
				return (StatusMessages.UnableToService, null);
			}
			if(user == null) 
			{
				return (StatusMessages.UnauthorizedAccess, null);
			}

			if(user.credentials.Any(c => c.id == id))
			{
				DomainModel.Password password = user.credentials.Find(c => c.id == id).password;

				if (password == null)
				{
					return (StatusMessages.CredentialNotexist, null);
				}

				SymmetricKey key = dataMapper.ConvertPasswordToSymmetricKey(password);
				string decryptedpassword = symmetricEncryption.DecryptString(key.password, key.aad, configuration);
				return (StatusMessages.Ok, new DTO.Password.Password() { password = decryptedpassword });
			}

			else
			{
				user = null;
				try
				{
					user = _dbContext.Users.Include(u => u.folders)
											.Include(u => u.folders).ThenInclude(pf => pf.credentials)
											.Include(u => u.folders).ThenInclude(pf => pf.credentials).ThenInclude(c => c.password)
											.FirstOrDefault(u => u.Id == _userid.ToString());
				}
				catch
				{
					return (StatusMessages.UnableToService,null);
				}

				if(user == null)
				{
					return(StatusMessages.UnauthorizedAccess,null);
				}

				DomainModel.PersonalFolder personalFolder = user.folders.Find(pf => pf.id == parentid);

				if(personalFolder == null)
				{
					return(StatusMessages.PersonalFolderNotexist,null);
				}

				DomainModel.Credential credential = personalFolder.credentials.Find(c => c.id == id);
				if(credential == null)
				{
					return (StatusMessages.CredentialNotexist,null);
				}

				if(credential.password == null)
				{
					return (StatusMessages.CredentialNotexist,null);
				}
				SymmetricKey key = dataMapper.ConvertPasswordToSymmetricKey(credential.password);
				string decryptedpassword = symmetricEncryption.DecryptString(key.password, key.aad, configuration);
				return (StatusMessages.Ok, new DTO.Password.Password() { password = decryptedpassword });
			}
		}
	}
}
