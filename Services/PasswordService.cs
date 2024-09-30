using DataAccessLayerDB;
using DataMapper;
using DomainModel;
using DTOModel.PasswordDTO;
using EncryptionLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TransitionObjectMapper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthenticationLayer;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Services
{
    public class PasswordService
    {

		public PasswordDTO GetCredentialPasswordById(PSDBContext _dbContext, SymmetricEncryption symmetricEncryption, IConfiguration configuration, PasswordDataMapper dataMapper, PasswordString passwordPostRequest, JwtTokenManager jwtTokenManager, string jwtToken)
		{
			if (jwtTokenManager.GetUserID(jwtToken, out Guid _userid))
			{
				User LoggedUser = _dbContext.Users.Include(u => u.teams)
												  .Include(u => u.teams).ThenInclude(t => t.credentials)
												  .Include(u => u.teams).ThenInclude(t => t.credentials).ThenInclude(c => c.password)
												  .FirstOrDefault(u => u.Id == _userid.ToString());

				if(LoggedUser != null)
				{
					Team team = LoggedUser.teams.FirstOrDefault(c => c.id == passwordPostRequest.parentid);
					if (team != null)
					{
						Credential credential = team.credentials.FirstOrDefault(cr => cr.id == passwordPostRequest.id);
						if(credential != null)
						{
							if( credential.password != null)
							{
								SymmetricKey key = dataMapper.ConvertPasswordToSymmetricKey(credential.password);
								string decryptedpassword = symmetricEncryption.DecryptString(key.password, key.aad, configuration);
								return new PasswordDTO() { password = decryptedpassword };
							}
							return new PasswordDTO();
						}
						return new PasswordDTO();
					}
					return new PasswordDTO();
				}
				return new PasswordDTO();
			}
			return new PasswordDTO();


		}

		public PasswordDTO GetCertificatePasswordById(PSDBContext _dbContext, SymmetricEncryption symmetricEncryption, IConfiguration configuration, PasswordDataMapper dataMapper, PasswordString passwordPostRequest, JwtTokenManager jwtTokenManager, string jwtToken)
		{
			if (jwtTokenManager.GetUserID(jwtToken, out Guid _userid))
			{
				User LoggedUser = _dbContext.Users.Include(u => u.teams)
												  .Include(u => u.teams).ThenInclude(t => t.client)
												  .Include(u => u.teams).ThenInclude(t => t.certificates)
												  .Include(u => u.teams).ThenInclude(t => t.certificates).ThenInclude(c => c.password)
												  .FirstOrDefault(u => u.Id == _userid.ToString());

				if (LoggedUser != null)
				{
					Team team = LoggedUser.teams.FirstOrDefault(c => c.id == passwordPostRequest.parentid);
					if (team != null)
					{
						Certificate credential = team.certificates.FirstOrDefault(cr => cr.id == passwordPostRequest.id);
						if (credential != null)
						{
							if (credential.password != null)
							{
								SymmetricKey key = dataMapper.ConvertPasswordToSymmetricKey(credential.password);
								string decryptedpassword = symmetricEncryption.DecryptString(key.password, key.aad, configuration);
								return new PasswordDTO() { password = decryptedpassword };
							}
							return new PasswordDTO();
						}
						return new PasswordDTO();
					}
					return new PasswordDTO();
				}
				return new PasswordDTO();
			}
			return new PasswordDTO();


		}

		public PasswordDTO GetPersonalPasswordById(PSDBContext _dbContext, SymmetricEncryption symmetricEncryption, IConfiguration configuration, PasswordDataMapper dataMapper, PasswordString passwordPostRequest, JwtTokenManager jwtTokenManager, string jwtToken)
		{
			if(jwtTokenManager.GetUserID(jwtToken, out Guid _userid))
			{

				if(_dbContext.Users.Include(u => u.folders).FirstOrDefault(u => u.Id == _userid.ToString()).folders.Any(pf => pf.id == passwordPostRequest.parentid))
				{
					Credential credential = _dbContext.PersonalFolders.Include(pf => pf.credentials)
																	  .Include(pf => pf.credentials).ThenInclude(c => c.password)
																	  .FirstOrDefault(pf => pf.id == passwordPostRequest.parentid)
																	  .credentials.FirstOrDefault(c => c.id == passwordPostRequest.id);
					
					if(credential != null)
					{
						if(credential.password != null)
						{
							SymmetricKey key = dataMapper.ConvertPasswordToSymmetricKey(credential.password);
							string decryptedpassword = symmetricEncryption.DecryptString(key.password, key.aad, configuration);
							return new PasswordDTO() { password = decryptedpassword };
						}
						return null;
					}
					return null;
				}
				else
				{
					User user = _dbContext.Users.Include(u => u.credentials)
												.Include(u => u.credentials).ThenInclude(c => c.password)
												.FirstOrDefault(u => u.Id == _userid.ToString());

					if(user != null)
					{
						Credential credential = _dbContext.Credentials.Include(c => c.password).FirstOrDefault(c => c.id == passwordPostRequest.id);
						if(credential != null)
						{
							if (credential.password != null)
							{
								SymmetricKey key = dataMapper.ConvertPasswordToSymmetricKey(credential.password);
								string decryptedpassword = symmetricEncryption.DecryptString(key.password, key.aad, configuration);
								return new PasswordDTO() { password = decryptedpassword };
							}
							return null;
						}
						return null;
					}
					return null;
				}
				return null;
			}
			return null;
		}
	}
}
