
using DataAccessLayerDB;
using DomainModel.DB;
using DTOModel;
using DTOModel.CredentialDTO;
using DTOModel.PersonalDTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Services
{
    public  class PersonalService
    {
        private readonly PSDBContext _dbContext;

        public PersonalService(PSDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<PersonalFolderDTO> GetPersonalFoldersByUserid(Guid userId)
        {
            UserDBDM user  = _dbContext.Users.Include(u => u.folders).Where(u => u.id == userId).First();
            List<PersonalFolderDTO> foldereList = new List<PersonalFolderDTO>();
            foreach(PersonalFolderDBDM folder in user.folders)
            {
                foldereList.Add(new PersonalFolderDTO()
                {
                    name = folder.name,
                    id = folder.id,
                });
            }
            return foldereList;
        }

        public List<PersonalFolderDTO> GetCredentialsFolderByFoderID(Guid userid, Guid folderID)
        {
            return new List<PersonalFolderDTO>();
        }
        public SetStatus AddPersonalFolder(string newPersonalFolderName, Guid userid)
        {
            PersonalFolderDBDM newPersonalFolder  = new PersonalFolderDBDM();
            newPersonalFolder.name = newPersonalFolderName;
            newPersonalFolder.id = Guid.NewGuid();

            UserDBDM user = _dbContext.Users.Include(u => u.folders).Where(u => u.id == userid).First();

            if (user != null) 
            {
                if( user.folders.Count > 0)
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
                    Console.WriteLine("folders" + string.Join(',',user.folders.Select(f => f.name)));
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
    
        public SetStatus AddPersonalCredential(PostPersonalCredentialDTO postPersonalCredential, Guid userid)
        {
            try
            {

                UserDBDM user = _dbContext.Users.Include(u => u.folders).ThenInclude(pf => pf.credentials).Single(u => u.id == userid);

                PersonalFolderDBDM personalFolder = user.folders.Single(pf => pf.id == postPersonalCredential.personalFolderId);

                CredentialDBDM newPersonalCredential = new CredentialDBDM();
                newPersonalCredential.domain = postPersonalCredential.domain;
                newPersonalCredential.username = postPersonalCredential.username;
                newPersonalCredential.password = new PasswordDBDM() { password = postPersonalCredential.password, id = Guid.NewGuid(), createdate = DateTime.Now, updatedate = DateTime.Now };
                newPersonalCredential.remote = postPersonalCredential.remote;
                newPersonalCredential.email = postPersonalCredential.email;
                newPersonalCredential.note = postPersonalCredential.note;


                _dbContext.Credentials.Add(newPersonalCredential);
                if (personalFolder.credentials.Count > 0)
                {
                    personalFolder.credentials.Add(newPersonalCredential);
                }
                else
                {
                    personalFolder.credentials = [newPersonalCredential];
                }

                _dbContext.SaveChanges();
                return new SetStatus() { status = "OK" };
            }
            catch
            {
                return new SetStatus() { status = "KO" };
            }
            
        }
    }
}
