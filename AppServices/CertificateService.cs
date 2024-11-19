using DTO;
using DTO.Certificate;
using DTO.Team;
using DataAccessLayerDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using DomainModel;
using DataMapper;
using TransitionObjectMapper;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using LogginMessages;
using EncryptionLayer;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using System.Text.Json;


namespace AppServices
{
    public class CertificateService
    {
        public List<DTO.Certificate.Certificate> GetCertificatesByClintId(Guid clientid, Guid userid, CertificateDataMapper certificateDataMapper, PSDBContext _dbContext, ILogger _logger)
        {
            List<TeamCertificatesMap> allCertificates = new List<TeamCertificatesMap>();

			User user = _dbContext.Users.Include(u => u.teams)
										.Include(u => u.teams).ThenInclude(c => c.client)
										.AsSplitQuery()
										.Include(u => u.teams).ThenInclude(t => t.certificates)
										.AsSplitQuery()
										.Include(u => u.teams).ThenInclude(t => t.certificates).ThenInclude(cert => cert.password)
										.AsSplitQuery()
										.FirstOrDefault(u => u.Id == userid.ToString());
            if( user == null)
            {
				return new List<DTO.Certificate.Certificate>();
			}
			allCertificates = user.teams.Where(t => t.client.id == clientid && t.certificates.Count > 0).Select(t => new TeamCertificatesMap() { teamid = t.id, clientid = t.client.id, teamname = t.name, certificates = t.certificates }).ToList();
			return certificateDataMapper.ConvertCertificateListToCertificateDTOList(allCertificates);

		}

        public (StatusMessages,List<DTO.Certificate.Certificate>) UploadCertificate(IConfiguration configuration, PSDBContext _dbContext, SymmetricEncryption _symmetricEncryption, UploadCertificatecs uploadCertificate, Guid _userid, ILogger _logger)
        {
			DTO.Certificate.Certificate syncCertificate = new DTO.Certificate.Certificate();
			List<DTO.Certificate.Certificate> outputList = new List<DTO.Certificate.Certificate> ();
			(bool pairStatus, List<ClientTeamPair> pairList) = IsCertificateUploadAuthorization(uploadCertificate.team, _dbContext, _userid, _logger);
			if(!pairStatus)
			{
				return  (StatusMessages.AccessDenied,null);
			}
			List<IFormFile> files = new List<IFormFile>();
			files.Add(uploadCertificate.certfile);
			if(uploadCertificate.certpass == null && uploadCertificate.certkey != null)
			{
				files.Add(uploadCertificate.certkey);
				foreach (ClientTeamPair pair in pairList)
				{
					CertificateFile certfile = new CertificateFile();
					CertificateFile keyfile = new CertificateFile();
					certfile.id = Guid.NewGuid();
					certfile.currentFileName = certfile.id.ToString() + "." + uploadCertificate.certfile.FileName.Split(".")[1]; ;
					certfile.originalFileName = uploadCertificate.certfile.FileName;
					certfile.createdate = DateTime.Now;

					keyfile.id = Guid.NewGuid();
					keyfile.currentFileName = keyfile.id.ToString() + "." + uploadCertificate.certkey.FileName.Split(".")[1];
					keyfile.originalFileName = uploadCertificate.certkey.FileName;
					keyfile.createdate = DateTime.Now;

					if (!IsValidCertificateName(files))
					{
						_logger.LogInformation($"{DateTime.Now} - Invalid Certificate file name\n");
						_logger.LogDebug($"{DateTime.Now} - Invalid Certificate file names:  {string.Join(',', files.Select(f => f.Name).ToList())}\n");
						return (StatusMessages.InvalidName,null);
					}
					if (!TryPemCertificateValidation(configuration, uploadCertificate.certfile, uploadCertificate.certkey, certfile.currentFileName, keyfile.currentFileName, out X509Certificate2 validCertificate))
					{
						_logger.LogInformation($"{DateTime.Now} - Invalid construction of certificate using PEM files\n");
						return (StatusMessages.InvalidCertificate,null);
					}

					DomainModel.Certificate newCertificate = new DomainModel.Certificate();
					newCertificate.id = Guid.NewGuid();
					newCertificate.name = validCertificate.FriendlyName;
					newCertificate.issuedBy = validCertificate.Issuer;
					newCertificate.issuedTo = ConvertDNtoCN(validCertificate.Subject);
					newCertificate.friendlyname = validCertificate.FriendlyName;
					newCertificate.expirationDate = validCertificate.NotAfter;
					newCertificate.file = certfile;
					newCertificate.key = keyfile;
					newCertificate.createdate = DateTime.Now;
					newCertificate.updatedate = DateTime.Now;

					syncCertificate.id = newCertificate.id;
					syncCertificate.name = newCertificate.name;
					syncCertificate.issuedto = newCertificate.issuedTo;
					syncCertificate.issuedby = newCertificate.issuedBy;
					syncCertificate.friendlyname = newCertificate.friendlyname;
					syncCertificate.expirationdate = newCertificate.expirationDate;
					

					Client client = _dbContext.Clients.Include(c => c.teams).FirstOrDefault(c => c.id == pair.clientid);
					if (client == null)
					{
						_logger.LogInformation($"{DateTime.Now} - Clinet does not exist {pair.clientid}\n");
						continue;
					}

					DomainModel.Team team = client.teams.FirstOrDefault(t => t.id == pair.teamid);
					if (team == null)
					{
						_logger.LogInformation($"{DateTime.Now} - Team does not exist {pair.teamid}\n");
						continue;
					}

					if (team.credentials != null)
					{
						team.certificates.Add(newCertificate);
						syncCertificate.teamid = team.id;
					}
					else
					{
						team.certificates = [newCertificate];
						syncCertificate.teamid = team.id;
					}

					try
					{
						_dbContext.Teams.Update(team);
						_dbContext.CertificatesFile.Add(certfile);
						_dbContext.CertificatesFile.Add(keyfile);
						_dbContext.Certificates.Add(newCertificate);
						outputList.Add(syncCertificate);
					}
					catch (Exception ex)
					{
						return (StatusMessages.UnableToService, null);
					}

				}
				try
				{
					_dbContext.SaveChanges();
					return (StatusMessages.AddNewCertificate, outputList);
				}
				catch
				{
					return (StatusMessages.UnableToService, null);
				}
				
			}
			else if(uploadCertificate.certpass != null && uploadCertificate.certkey == null)
			{
				Console.WriteLine("Debug#2: ");
				foreach (ClientTeamPair pair in pairList)
				{
					CertificateFile certfile = new CertificateFile();
					certfile.id = Guid.NewGuid();
					certfile.currentFileName = certfile.id.ToString() + "." + uploadCertificate.certfile.FileName.Split(".")[1];
					certfile.originalFileName = uploadCertificate.certfile.FileName;
					certfile.createdate = DateTime.Now;

					if (!IsValidCertificateName(files))
					{
						_logger.LogInformation($"{DateTime.Now} - Invalid Certificate file name\n");
						_logger.LogDebug($"{DateTime.Now} - Invalid Certificate file names:  {string.Join(',', files.Select(f => f.Name).ToList())}\n");
						return (StatusMessages.InvalidName,null);
					}
					if (!TryCertificateValidation(configuration, uploadCertificate.certfile, certfile.currentFileName, uploadCertificate.certpass, out X509Certificate2 validCertificate))
					{
						_logger.LogInformation($"{DateTime.Now} - Invalid construction of certificate using cert file or cert password\n");
						return (StatusMessages.InvalidCertificate,null);
					}

					DomainModel.Certificate newCertificate = new DomainModel.Certificate();
					newCertificate.id = Guid.NewGuid();
					newCertificate.name = validCertificate.FriendlyName;
					newCertificate.issuedBy = ConvertDNtoCN(validCertificate.Issuer);
					newCertificate.issuedTo = ConvertDNtoCN(validCertificate.Subject);
					newCertificate.friendlyname = validCertificate.FriendlyName;
					newCertificate.expirationDate = validCertificate.NotAfter;
					newCertificate.file = certfile;
					newCertificate.createdate = DateTime.Now;
					newCertificate.updatedate = DateTime.Now;

					syncCertificate.id = newCertificate.id;
					syncCertificate.name = newCertificate.name;
					syncCertificate.issuedto = newCertificate.issuedTo;
					syncCertificate.issuedby = newCertificate.issuedBy;
					syncCertificate.friendlyname = newCertificate.friendlyname;
					syncCertificate.expirationdate = newCertificate.expirationDate;

					SymmetricKey key = _symmetricEncryption.EncryptString(uploadCertificate.certpass, configuration);

					newCertificate.password = new Password() { aad = key.aad, password = key.password, createdate = DateTime.Now, updatedate = DateTime.Now, id = Guid.NewGuid() };

					Client client = _dbContext.Clients.Include(c => c.teams).FirstOrDefault(c => c.id == pair.clientid);
					if (client == null)
					{
						_logger.LogInformation($"{DateTime.Now} - Clinet does not exist {pair.clientid}\n");
						continue;
					}

					DomainModel.Team team = client.teams.FirstOrDefault(t => t.id == pair.teamid);
					if (team == null)
					{
						_logger.LogInformation($"{DateTime.Now} - Team does not exist {pair.teamid}\n");
						continue;
					}
					if (team.credentials != null)
					{
						team.certificates.Add(newCertificate);
						syncCertificate.teamid = team.id;
					}
					else
					{
						team.certificates = [newCertificate];
						syncCertificate.teamid = team.id;
					}
					try
					{
						_dbContext.Teams.Update(team);
						_dbContext.Passwords.Add(newCertificate.password);
						_dbContext.CertificatesFile.Add(certfile);
						_dbContext.Certificates.Add(newCertificate);
					}
					catch
					{
						return (StatusMessages.UnableToService, null);
					}
					
				}
				try
				{
					_dbContext.SaveChanges();
					return (StatusMessages.AddNewCertificate,outputList);
				}
				catch
				{
					return(StatusMessages.UnableToService,null);
				}
			}
			else
			{
				_logger.LogCritical($"{DateTime.Now} - Invalid upload for constructing certificate\n");
				return (StatusMessages.InvalidCertificate,null);
			}
			
        }

		public bool IsValidCertificateName(List<IFormFile> files) //change logic
		{
			Regex regexname = new Regex(@"^[a-zA-Z0-9_-]*$");
			List<string> privatekeypostfix = ["pfx", "p12", "pem", "crt", "key"];
			return files.All(file => regexname.IsMatch(file.FileName.Split(".")[0]) && privatekeypostfix.Any(postfix => postfix == file.FileName.Split(".")[1]));
		}

		public bool ifItemExist(List<string> strings, string item) => strings.Any(s => s == item);

        public bool TrySaveCertificate(IConfiguration configuration, IFormFile cert, string certname) // change logic
        {
			try
			{
				using (Stream sw = System.IO.File.Create(configuration.GetSection("CertificateLocation").Value + "\\" + certname))
				{
					cert.CopyTo(sw);
					return true;
				}
			}
			catch
			{
			    Console.WriteLine("Problem saving certificate: {0}", certname);
			    return false;
			}
        }

		public bool TrySaveCertificate(IConfiguration configuration, IFormFile cert, IFormFile key, string certname, string keyname) // change logic
		{
			try
			{
				using (Stream sw = System.IO.File.Create(configuration.GetSection("CertificateLocation").Value + "\\" + certname))
				{
					cert.CopyTo(sw);
				}
				using (Stream sw = System.IO.File.Create(configuration.GetSection("CertificateLocation").Value + "\\" + keyname))
				{
					key.CopyTo(sw);
				}
				return true;
			}
			catch
			{
				Console.WriteLine("Problem saving certificate: {0}", certname);
				Console.WriteLine("Problem saving key: {0}", keyname);
				return false;
			}
		}

		public bool IsValidCertificate(IConfiguration configuration, string certname, string certpass, out X509Certificate2 _temp)
        {
			try
			{
				X509Certificate2 temp = new X509Certificate2(configuration.GetSection("CertificateLocation").Value + "\\" + certname, certpass);
				_temp = temp;
				return true;
			}
			catch
			{
				Console.WriteLine("Invalid Certificate file or password");
				_temp = null;
				return false;
			}
        }
		public bool IsValidPemCertificate(IConfiguration configuration, string certname, string keyname, out X509Certificate2 _temp)
		{
			try
			{
				X509Certificate2 temp = new X509Certificate2(configuration.GetSection("CertificateLocation").Value + "\\" + certname, configuration.GetSection("CertificateLocation").Value + "\\" + keyname);
				_temp = temp;
				return true;
			}
			catch
			{
				Console.WriteLine("Invalid Certificate file");
				_temp = null;
				return false;
			}
		}
	
        public bool DeleteCertificateFile(IConfiguration configuration,string certname)
        {
            if(File.Exists(configuration.GetSection("CertificateLocation").Value+"\\"+certname))
            {
                try
                {
					File.Delete(configuration.GetSection("CertificateLocation").Value + "\\" + certname);
                    return true;
				}
                catch(Exception ex)
                {
                    return false;
                }
            }
            return true;
        }
    
		public bool AddCertificateToTeams(List<ClientTeamPair> teams, PSDBContext _dbContext, DomainModel.Certificate cert, CertificateFile certfile) // change exceptions
		{
			List<DomainModel.Team> teamListToEdit = new List<DomainModel.Team>();
			for(int i=0; i<teams.Count; i++)
			{
				try
				{
					DomainModel.Certificate tempCert = cert;
					CertificateFile tempcertfile = certfile;
					teamListToEdit[i] = _dbContext.Clients.Include(c => c.teams).FirstOrDefault(c => c.id == teams[i].clientid).teams.FirstOrDefault(t => t.id == teams[i].teamid);
					tempCert.id = Guid.NewGuid();
					tempcertfile.id = Guid.NewGuid();
					tempCert.file = tempcertfile;
					if (teamListToEdit[i].certificates != null)
					{
						teamListToEdit[i].certificates.Add(cert);
					}
					else
					{
						teamListToEdit[i].certificates = [cert];
					}

				}
				catch
				{
					return false;
				}
			}
			_dbContext.UpdateRange(teamListToEdit);
			_dbContext.SaveChanges();
			return true;

		}

		public bool AddCertificateToTeams(List<ClientTeamPair> teams, PSDBContext _dbContext, DomainModel.Certificate cert, CertificateFile certfile, CertificateFile keyfile) //change missing logs
		{

			List<DomainModel.Team> teamListToEdit = new List<DomainModel.Team>();
			for(int i=0;i<teams.Count;i++)
			{
				try
				{
					DomainModel.Certificate tempCert = cert;
					CertificateFile tempcertfile = certfile;
					CertificateFile tempkeyfile = keyfile;

					Client client = _dbContext.Clients.Include(c => c.teams).FirstOrDefault(c => c.id == teams[i].clientid);
					if (client == null)
					{
						Console.WriteLine("Client does not exist");
						continue;
					}
					teamListToEdit[i] = client.teams.FirstOrDefault(t => t.id == teams[i].teamid);
					if (teamListToEdit[i] == null)
					{
						Console.WriteLine("Team does not exist");
						continue;
					}

					tempCert.id = Guid.NewGuid();
					tempcertfile.id = Guid.NewGuid();
					tempkeyfile.id = Guid.NewGuid();
					tempCert.file = tempcertfile;
					tempCert.key = tempkeyfile;
					if (teamListToEdit[i].certificates != null)
					{
						teamListToEdit[i].certificates.Add(cert);
					}
					else
					{
						teamListToEdit[i].certificates = [cert];
					}
				}
				catch
				{
					return false;
				}


			}
			_dbContext.Update(teamListToEdit);
			_dbContext.SaveChanges();
			return true;
		}

		public bool TryCertificateValidation(IConfiguration configuration, IFormFile certfile, string certname, string certpass, out X509Certificate2 _out)
		{
			if(TrySaveCertificate(configuration, certfile, certname))
			{
				if(IsValidCertificate(configuration,certname,certpass,out X509Certificate2 validCertificate))
				{
					_out = validCertificate;
					return true;
				}
				_out = null;
				return false;
			}
			_out = null;
			return false;
		}
		public bool TryPemCertificateValidation(IConfiguration configuration, IFormFile certfile, IFormFile keyfile, string certname, string keyname, out X509Certificate2 _out)
		{
			if (TrySaveCertificate(configuration, certfile,keyfile, certname,keyname))
			{
				if (IsValidPemCertificate(configuration, certname, keyname, out X509Certificate2 validCertificate))
				{
					_out = validCertificate;
					return true;
				}
				_out = null;
				return false;
			}
			_out = null;
			return false;
		}
	
		public string ConvertDNtoCN(string dn)
		{
			return dn.Split(",").Aggregate("",(name, parts) => name += parts.Split("=")[1] +".");

		}
	
		public StatusMessages Delete(Guid id, Guid teamid, PSDBContext _dbContext, IConfiguration _configuration,Guid _userid)
		{
			User user = _dbContext.Users.Include(u => u.teams)
											.Include(u => u.teams).ThenInclude(t => t.certificates)
											.FirstOrDefault(u => u.Id == _userid.ToString() && u.teams.Any(t => t.id == teamid && t.certificates.Any(c => c.id == id)));
				if(user == null)
				{
					return StatusMessages.UnauthorizedAccess;
				}
				DomainModel.Certificate removeCertificate = _dbContext.Certificates.Include(c => c.password)
														.Include(c => c.file)
														.Include(c => c.key)
														.FirstOrDefault(c => c.id == id);


				DeleteCertificateFile(_configuration, removeCertificate.file.currentFileName);
				if (removeCertificate.key != null)
				{
					DeleteCertificateFile(_configuration, removeCertificate.key.currentFileName);
					_dbContext.CertificatesFile.Remove(removeCertificate.key);

				}
				if (removeCertificate.password != null)
				{
					_dbContext.Passwords.Remove(removeCertificate.password);
				}

				user.teams.FirstOrDefault(t => t.id == teamid).certificates.Remove(removeCertificate);
				_dbContext.CertificatesFile.Remove(removeCertificate.file);
				_dbContext.Certificates.Remove(removeCertificate);
				_dbContext.Teams.Update(user.teams.FirstOrDefault(t => t.id == teamid));
				_dbContext.SaveChanges();

				return StatusMessages.DeleteCertificate;

		}
	
	
		public ResponseDownloadCertificate DownloadCertificate([FromServices] PSDBContext _dbContext, [FromServices] IConfiguration _configuration,Guid _userid, RequestDownloadCertificate downloadCertificate)
		{
				User user = _dbContext.Users.Include(u => u.teams)
											.Include(u => u.teams).ThenInclude(t => t.certificates)
											.AsSplitQuery()
											.Include(u => u.teams).ThenInclude(t => t.certificates).ThenInclude(c => c.file)
											.AsSplitQuery()
											.FirstOrDefault(u => u.Id == _userid.ToString() && u.teams.Any(t => t.id == downloadCertificate.teamId && t.certificates.Any(c => c.id == downloadCertificate.certificateId)));
				if(user == null)
				{
					return null;
				}

				DomainModel.Certificate certificate = user.teams.FirstOrDefault(t => t.id == downloadCertificate.teamId)
								  .certificates.FirstOrDefault(c => c.id == downloadCertificate.certificateId);
				if (certificate == null)
				{
					return null;
				}

				ResponseDownloadCertificate output = new ResponseDownloadCertificate();
				output.FileContent = new FileContentResult(System.IO.File.ReadAllBytes(_configuration.GetSection("CertificateLocation").Value + "\\" + certificate.file.currentFileName), "application/octet-stream");
				output.filename = certificate.file.originalFileName;
				return output;

		}

		public ResponseDownloadCertificate DownloadCertificateKey([FromServices] PSDBContext _dbContext, [FromServices] IConfiguration _configuration, Guid _userid, RequestDownloadCertificate downloadCertificate)
		{
			User user = _dbContext.Users.Include(u => u.teams)
										.AsSplitQuery()
										.Include(u => u.teams).ThenInclude(t => t.certificates)
										.AsSplitQuery()
										.Include(u => u.teams).ThenInclude(t => t.certificates).ThenInclude(c => c.key)
										.AsSplitQuery()
										.FirstOrDefault(u => u.Id == _userid.ToString() && u.teams.Any(t => t.id == downloadCertificate.teamId && t.certificates.Any(c => c.id == downloadCertificate.certificateId)));
			if (user == null)
			{
				return null;
			}
			DomainModel.Certificate certificate = user.teams.FirstOrDefault(t => t.id == downloadCertificate.teamId)
							  .certificates.FirstOrDefault(c => c.id == downloadCertificate.certificateId);
			if (certificate == null)
			{
				return null;
			}
			if (certificate.key == null)
			{
				return null;
			}
			ResponseDownloadCertificate output = new ResponseDownloadCertificate();
			output.FileContent = new FileContentResult(System.IO.File.ReadAllBytes(_configuration.GetSection("CertificateLocation").Value + "\\" + certificate.key.currentFileName), "application/octet-stream");
			output.filename = certificate.key.originalFileName;
			return output;

		}

		public (bool status, List<ClientTeamPair> pairs)IsCertificateUploadAuthorization(List<string> clientteampairs, PSDBContext _dbContext, Guid _userid, ILogger _logger)
		{
			List<ClientTeamPair> output = new List<ClientTeamPair>();

			User loggedinUser = new User();

			try
			{
				loggedinUser = _dbContext.Users.Include(u => u.teams)
											   .Include(u => u.teams).ThenInclude(c => c.client)
											   .FirstOrDefault(u => u.Id == _userid.ToString());
			}
			catch (Exception ex)
			{
				_logger.LogCritical(DatabaseLog.DBConnectionLog(ex.ToString()));
			}

			if (loggedinUser == null)
			{
				_logger.LogDebug(EntityLog.NotFound("User", loggedinUser.Id));
				return (false, new List<ClientTeamPair>());
			}

			foreach (string pair in clientteampairs)
			{

				ClientTeamPair _pair = new ClientTeamPair();
				try
				{
					_pair = JsonSerializer.Deserialize<ClientTeamPair>(pair);
				}
				catch
				{
					_logger.LogDebug(EntityLog.InvalidJsonString(pair));
					continue;
				}

				try
				{
					DomainModel.Team team = loggedinUser.teams.FirstOrDefault(c => c.id == _pair.teamid);
					if (team != null)
					{
						output.Add(_pair);
					}
					else
					{
						_logger.LogDebug(EntityLog.NotFound("Team", _pair.teamid.ToString()));
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(DatabaseLog.DBConnectionLog(ex.ToString()));
				}

			}
			return (true, output);

		}
	}

}

