using DTOModel.CredentialDTO;
using DTOModel;
using DomainModel.DB;
using DataAccessLayerDB;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
namespace Services
{
    public  class CredentialService
    {
        private readonly PSDBContext _dbContext;
        public CredentialService(PSDBContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public List<CredentialDTO> GetCredentialsByUserIdClintId(Guid clientid, Guid userid)
        {
            //User splitQuery() to increase performance
            List<TeamCredentialsMap> teamCredentials = _dbContext.Users.Include(u => u.teams)
                                       .Include(u => u.teams).ThenInclude(t => t.client)
                                       .Include(u => u.teams).ThenInclude(t => t.credentials)
                                       .Where(u => u.id == userid).First()
                                       .teams.Where(t => t.client.id == clientid)
                                       .Select(t => new TeamCredentialsMap() {teamid = t.id,  teamname= t.name, credentials= t.credentials }).ToList();            

            return ConvertCredentialDBDMtoDTO(teamCredentials);  
        }

        public List<CredentialDTO> GetCredentialsByUserId(Guid userId, Guid folderid)
        {
            List<CredentialDTO> personalCredentialList = new List<CredentialDTO> ();
            UserDBDM user = _dbContext.Users.Include(u => u.folders).ThenInclude(pf => pf.credentials).Where(u => u.id == userId && u.folders.Any(pf => pf.id == folderid)).First();
            if(user != null)
            {
                foreach(CredentialDBDM credential in user.folders.Single(pf => pf.id == folderid).credentials)
                {
                    personalCredentialList.Add(new CredentialDTO()
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

        public SetStatus AddCredential(PostCredentialDTO postCredential)
        {
            Console.WriteLine(JsonSerializer.Serialize(postCredential));
            CredentialDBDM newCredential = new CredentialDBDM();

            newCredential.id = Guid.NewGuid();
            newCredential.domain = postCredential.domain;
            newCredential.password = new PasswordDBDM() { id = Guid.NewGuid(), password= postCredential.password };
            newCredential.username = postCredential.username;
            newCredential.email = postCredential.email;
            newCredential.remote = postCredential.remote;
            newCredential.note = postCredential.note;
            newCredential.createdate = DateTime.Now;
            newCredential.updatedate = DateTime.Now;
            if(postCredential.teams.Count > 0 && postCredential.teams != null) 
            {
                foreach (var team in postCredential.teams)
                {
                    Console.WriteLine("Team Name"+_dbContext.Teams.Find(team.teamid).name);
                    TeamDBDM tempteam = _dbContext.Teams.Find(team.teamid);
                    if(tempteam.credentials != null)
                    {
                        tempteam.credentials.Add(newCredential);
                        _dbContext.Teams.Update(tempteam);
                    }
                    else
                    {
                        tempteam.credentials = [newCredential];
                        _dbContext.Teams.Update(tempteam);
                    }
                    
                    
                }
                _dbContext.Passwords.Add(newCredential.password);
                _dbContext.Credentials.Add(newCredential);

                _dbContext.SaveChanges();
            }
            else
            {
                _dbContext.Passwords.Add(newCredential.password);
                _dbContext.Credentials.Add(newCredential);

                _dbContext.SaveChanges();
            }


            return new SetStatus() { status = "OK" };
        }
    
        public List<CredentialDTO> ConvertCredentialDBDMtoDTO(List<TeamCredentialsMap> credentials)
        {
            List<CredentialDTO> output = new List<CredentialDTO>();

            foreach(TeamCredentialsMap teamCredentialMap in credentials)
            {

                foreach(CredentialDBDM teamcred in teamCredentialMap.credentials)
                {
                    output.Add(new CredentialDTO
                    {
                        id = teamcred.id,
                        username = teamcred.username,
                        domain = teamcred.domain,
                        password = "******",
                        email = teamcred.email,
                        note = teamcred.note,
                        remote = teamcred.remote,
                        teamname = teamCredentialMap.teamname,
                        teamid = teamCredentialMap.teamid,
                    });
                }
            }
            return output;
        }
    
        public SetStatus DeleteCredential(Guid userid,Guid teamid, Guid credentialid)
        {
            try
            {
                CredentialDBDM credential = _dbContext.Users.Include(u => u.teams)
                                                            .Include(u => u.teams).ThenInclude(t => t.credentials)
                                                            .Include(u => u.teams).ThenInclude(t => t.credentials).ThenInclude(c => c.password)
                                                            .Single(u => u.id == userid).teams
                                                            .Single(t => t.id == teamid).credentials
                                                            .Single(c => c.id == credentialid);
            
                _dbContext.Credentials.Remove(credential);
                _dbContext.SaveChanges();
                return new SetStatus() { status = "OK" };
            }
            catch
            {
                return new SetStatus() { status = "KO" };
            }

        }
    
        public SetStatus GiveCredential(PostGiveCredentialDTO _giveCredential)
        {
            try
            {
                CredentialDBDM newCredential = new CredentialDBDM();
                newCredential.id = Guid.NewGuid();
                newCredential.domain = _giveCredential.domain;
                newCredential.username = _giveCredential.username;
                newCredential.password = new PasswordDBDM() { password = _giveCredential.password, id = Guid.NewGuid(), createdate = DateTime.Now, updatedate = DateTime.Now };
                newCredential.email = _giveCredential.email;
                newCredential.createdate = DateTime.Now;
                newCredential.updatedate = DateTime.Now;
                newCredential.remote = _giveCredential.remote;
                newCredential.note = _giveCredential.note;

                _dbContext.Credentials.Add(newCredential);

                List<UserDBDM> users = _dbContext.Users.Where(u => _giveCredential.userids.Any(uid => uid == u.id)).ToList();
                foreach (UserDBDM user in users)
                {
                    if (user.credentials == null || user.credentials.Count == 0)
                    {
                        user.credentials = [newCredential];
                        
                    }
                    else
                    {
                        user.credentials.Add(newCredential);
                    }

                }

                List<TeamDBDM> teams = _dbContext.Teams.Where(t => _giveCredential.teamids.Any(tid => tid == t.id)).ToList();
                foreach (TeamDBDM team in teams)
                {
                    if (team.credentials == null || team.credentials.Count == 0)
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
    }
}
