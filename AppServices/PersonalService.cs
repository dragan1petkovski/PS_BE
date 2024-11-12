using DataAccessLayerDB;
using DomainModel;
using DTO.Credential;
using DTO.Personal;
using EncryptionLayer;
using LogginMessages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.X509;
using TransitionObjectMapper;

namespace AppServices
{
    public  class PersonalService
    {
        public (StatusMessages, List<DTO.Personal.PersonalFolder>) GetPersonalFolders(Guid userid, PSDBContext _dbContext)
        {
			User user = null;
			try
			{
				user = _dbContext.Users.Include(u => u.folders).Where(u => u.Id == userid.ToString()).First();
			}
			catch
			{
				return (StatusMessages.UnableToService,null);
			}
			if(user == null)
			{
				return (StatusMessages.UnauthorizedAccess,null);
			}
			List<DTO.Personal.PersonalFolder> folderList = new List<DTO.Personal.PersonalFolder>();
			foreach (DomainModel.PersonalFolder folder in user.folders)
			{
				folderList.Add(new DTO.Personal.PersonalFolder()
				{
					name = folder.name,
					id = folder.id,
				});
			}
			return (StatusMessages.Ok, folderList);
		}
        
		public (StatusMessages statusCode, DTO.Personal.PersonalFolder) GetPersonalFolderById(Guid userid, Guid id, PSDBContext _dbContext)
		{
			User user = null;
			try
			{
				user = _dbContext.Users.Include(u => u.folders).Where(u => u.Id == userid.ToString()).First();
			}
			catch
			{
				return (StatusMessages.UnableToService, null);
			}
			if (user == null)
			{
				return (StatusMessages.UnauthorizedAccess, null);
			}
			DomainModel.PersonalFolder folder = user.folders.Find(f => f.id == id);
			if(folder == null)
			{
				return (StatusMessages.PersonalFolderNotexist, null);
			}
			return (StatusMessages.Ok, new DTO.Personal.PersonalFolder() { id = folder.id, name = folder.name});
		}

		public StatusMessages AddPersonalFolder(string newPersonalFolderName, Guid userid, PSDBContext _dbContext)
        {

			User user = _dbContext.Users.Include(u => u.folders).FirstOrDefault(u => u.Id == userid.ToString());
			if (user == null)
			{
				return StatusMessages.UnauthorizedAccess;
			}

			DomainModel.PersonalFolder newPersonalFolder = new DomainModel.PersonalFolder();
			newPersonalFolder.name = newPersonalFolderName;
			newPersonalFolder.id = Guid.NewGuid();
			if (user.folders.Count > 0)
			{
				_dbContext.PersonalFolders.Add(newPersonalFolder);
				user.folders.Add(newPersonalFolder);
				user.updatedate = DateTime.Now;
				_dbContext.Update(user);
				_dbContext.SaveChanges();
				return StatusMessages.AddNewPersonalFolder;
			}
			else
			{
				_dbContext.PersonalFolders.Add(newPersonalFolder);
				user.folders = [newPersonalFolder];
				user.updatedate = DateTime.Now;
				_dbContext.Update(user);
				_dbContext.SaveChanges();
				return StatusMessages.AddNewPersonalFolder;
			}
		}

		public StatusMessages Update(Guid userid, Guid personalFolderId, string newPersonalFolderName, PSDBContext _dbContext)
		{
			DomainModel.User loggedUser = null;
			try
			{
				loggedUser = _dbContext.Users.Include(u => u.folders).FirstOrDefault(u => u.Id == userid.ToString());
			}
			catch
			{
				return StatusMessages.UnableToService;
			}

			if (loggedUser == null)
			{
				return StatusMessages.AccessDenied;
			}

			DomainModel.PersonalFolder folder = loggedUser.folders.Find(f => f.id == personalFolderId);
			if (folder == null)
			{
				return StatusMessages.PersonalFolderNotexist;
			}
			folder.name = newPersonalFolderName;
			try
			{
				_dbContext.PersonalFolders.Update(folder);
				_dbContext.SaveChanges();
				return StatusMessages.UpdatePersonalFolder;
			}
			catch
			{
				return StatusMessages.UnableToService;
			}

		}
		public StatusMessages Delete(Guid personalFolderId,Guid userid, PSDBContext _dbContext)
		{
			User user = _dbContext.Users.Include(u => u.folders)
										.Include(u => u.folders).ThenInclude(pf => pf.credentials)
										.Include(u => u.folders).ThenInclude(pf => pf.credentials).ThenInclude(c => c.password)
										.FirstOrDefault(u => u.Id == userid.ToString() && u.folders.Any(pf => pf.id == personalFolderId));

			if (user == null)
			{
				return StatusMessages.UnauthorizedAccess;
			}

			DomainModel.PersonalFolder personalFolder = user.folders.FirstOrDefault(pf => pf.id == personalFolderId);
			if (personalFolder == null)
			{
				return StatusMessages.PersonalFolderNotexist;
			}


			List<Password> passwordsToRemove = new List<Password>();
			personalFolder.credentials.ForEach(c => passwordsToRemove.Add(c.password));

			List<DomainModel.Credential> credentialsToRemove = personalFolder.credentials;

			user.folders.Remove(personalFolder);
			_dbContext.Passwords.RemoveRange(passwordsToRemove);
			_dbContext.Credentials.RemoveRange(credentialsToRemove);
			_dbContext.PersonalFolders.Remove(personalFolder);
			_dbContext.SaveChanges();
			return StatusMessages.DeletePersonalFolder;
		}

	}
}
