using AuthenticationLayer;
using DataAccessLayerDB;
using DomainModel;
using DTOModel;
using DTOModel.CredentialDTO;
using DTOModel.PersonalDTO;
using EncryptionLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Bcpg;
using System.Text.Json;
using TransitionObjectMapper;

namespace Services
{
    public  class PersonalService
    {
        public List<PersonalFolderDTO> GetPersonalFoldersByUserid(PSDBContext _dbContext, JwtTokenManager jwtTokenManager, string jwt)
        {
            if(jwtTokenManager.GetUserID(jwt,out Guid userid))
            {
				User user = _dbContext.Users.Include(u => u.folders).Where(u => u.Id == userid.ToString()).First();
				List<PersonalFolderDTO> foldereList = new List<PersonalFolderDTO>();
				foreach (PersonalFolder folder in user.folders)
				{
					foldereList.Add(new PersonalFolderDTO()
					{
						name = folder.name,
						id = folder.id,
					});
				}
				return foldereList;
			}
            return new List<PersonalFolderDTO>();
        }

        public List<PersonalFolderDTO> GetCredentialsFolderByFoderID(Guid folderID)
        {
            return new List<PersonalFolderDTO>();
        }
        public SetStatus AddPersonalFolder(string newPersonalFolderName, PSDBContext _dbContext, JwtTokenManager jwtTokenManager, string jwt)
        {

            if(jwtTokenManager.GetUserID(jwt, out Guid userid))
            {
				User user = _dbContext.Users.Include(u => u.folders).FirstOrDefault(u => u.Id == userid.ToString());
				if (user != null)
				{
					PersonalFolder newPersonalFolder = new PersonalFolder();
					newPersonalFolder.name = newPersonalFolderName;
					newPersonalFolder.id = Guid.NewGuid();
					if (user.folders.Count > 0)
					{
						_dbContext.PersonalFolders.Add(newPersonalFolder);
						user.folders.Add(newPersonalFolder);
						user.updatedate = DateTime.Now;
						_dbContext.Update(user);
						_dbContext.SaveChanges();
						return new SetStatus() { status = "OK" };
					}
					else
					{
						_dbContext.PersonalFolders.Add(newPersonalFolder);
						user.folders = [newPersonalFolder];
						user.updatedate = DateTime.Now;
						_dbContext.Update(user);
						_dbContext.SaveChanges();
						return new SetStatus() { status = "OK" };
					}


				}
				else
				{
					return new SetStatus() { status = "KO" };
				}
			}

			return new SetStatus() { status = "KO" };
		}
        public SetStatus AddPersonalCredential(PostPersonalCredentialDTO postPersonalCredential, PSDBContext _dbContext, JwtTokenManager jwtTokenManager, string jwt, SymmetricEncryption _symmetricEncryption, IConfiguration _configuration)
        {
            if(jwtTokenManager.GetUserID(jwt, out Guid userid))
            {
				try
				{
					User user = _dbContext.Users.Include(u => u.folders)
												.Include(u => u.folders).ThenInclude(pf => pf.credentials)
												.FirstOrDefault(u => u.Id == userid.ToString());

                    if (user != null)
                    {
						PersonalFolder personalFolder = user.folders.FirstOrDefault(pf => pf.id == postPersonalCredential.personalFolderId);
						if(personalFolder != null)
						{
							Credential newPersonalCredential = new Credential();
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

							_dbContext.Passwords.Add(newPersonalCredential.password);
							_dbContext.Credentials.Add(newPersonalCredential);
							_dbContext.PersonalFolders.Update(personalFolder);

							_dbContext.SaveChanges();
							return new SetStatus() { status = "OK" };
						}
						else
						{
							Credential newPersonalCredential = new Credential();
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
							_dbContext.Users.Update(user);
							_dbContext.SaveChanges();
							return new SetStatus() { status = "OK" };
						}
						
						return new SetStatus() { status = "KO" };
					}
					return new SetStatus() { status = "KO" };
				}
				catch
				{
					return new SetStatus() { status = "KO" };
				}
			}
			return new SetStatus() { status = "KO" };

		}
    
		public SetStatus Delete(Guid personalFolderId, PSDBContext _dbContext, JwtTokenManager jwtTokenManager, string jwt)
		{
			if(jwtTokenManager.GetUserID(jwt, out Guid userid))
			{
				User user = _dbContext.Users.Include(u => u.folders)
											.Include(u => u.folders).ThenInclude(pf => pf.credentials)
											.Include(u => u.folders).ThenInclude(pf => pf.credentials).ThenInclude(c => c.password)
											.FirstOrDefault(u => u.Id ==userid.ToString() && u.folders.Any(pf => pf.id == personalFolderId));

				if(user != null)
				{
					PersonalFolder personalFolder = user.folders.FirstOrDefault(pf => pf.id == personalFolderId);
					if (personalFolder != null)
					{
						List<Password> passwordsToRemove = new List<Password>();
						personalFolder.credentials.ForEach(c => passwordsToRemove.Add(c.password));

						List<Credential> credentialsToRemove = personalFolder.credentials;

						user.folders.Remove(personalFolder);
						_dbContext.Passwords.RemoveRange(passwordsToRemove);
						_dbContext.Credentials.RemoveRange(credentialsToRemove);
						_dbContext.PersonalFolders.Remove(personalFolder);
						_dbContext.SaveChanges();
						return new SetStatus() { status = "OK" };
					}
					return new SetStatus() { status = "KO" };
				}
				return new SetStatus() { status = "KO" };
			}
			return new SetStatus() { status = "KO" };
		}


		public SetStatus Delete(PersonalFolderList personalFolderIds, PSDBContext _dbContext, JwtTokenManager jwtTokenManager, string jwt)
		{
			if (jwtTokenManager.GetUserID(jwt, out Guid userid))
			{
				User user = _dbContext.Users.Include(u => u.folders)
											.Include(u => u.folders).ThenInclude(pf => pf.credentials)
											.Include(u => u.folders).ThenInclude(pf => pf.credentials).ThenInclude(c => c.password)
											.FirstOrDefault(u => u.Id == userid.ToString() && u.folders.Any(pf => personalFolderIds.personalFolderIds.Any(_pf => _pf == pf.id)));

				if (user != null)
				{
					foreach(Guid personalFolderId in personalFolderIds.personalFolderIds)
					{
						PersonalFolder personalFolder = user.folders.FirstOrDefault(pf => pf.id == personalFolderId);
						if (personalFolder != null)
						{
							List<Password> passwordsToRemove = new List<Password>();
							personalFolder.credentials.ForEach(c => passwordsToRemove.Add(c.password));

							List<Credential> credentialsToRemove = personalFolder.credentials;

							user.folders.Remove(personalFolder);
							_dbContext.Passwords.RemoveRange(passwordsToRemove);
							_dbContext.Credentials.RemoveRange(credentialsToRemove);
							_dbContext.PersonalFolders.Remove(personalFolder);
							_dbContext.SaveChanges();
							return new SetStatus() { status = "OK" };
						}
					}

					return new SetStatus() { status = "KO" };
				}
				return new SetStatus() { status = "KO" };
			}
			return new SetStatus() { status = "KO" };
		}

	}
}
