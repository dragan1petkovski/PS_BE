using DTOModel;
using DTOModel.TeamDTO;
using DataAccessLayerDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using DomainModel;
using DataMapper;
using AuthenticationLayer;
using TransitionObjectMapper;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using DTOModel.TeamDTO;
using EncryptionLayer;
using System.Text.RegularExpressions;
using System.IO.Compression;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using System.Globalization;


namespace Services
{
    public class CertificateService
    {
        public List<CertificateDTO> GetCertificatesByClintId(Guid clientid, CertificateDataMapper certificateDataMapper,JwtTokenManager jwtTokenManager, string jwt, PSDBContext _dbContext)
        {
            List<TeamCertificatesMap> allCertificates = new List<TeamCertificatesMap>();
            if(jwtTokenManager.GetUserID(jwt, out Guid userid))
            {
                User user = _dbContext.Users.Include(u => u.teams)
                                            .Include(u => u.teams).ThenInclude(c => c.client)
											.Include(u => u.teams).ThenInclude(t => t.certificates)
											.Include(u => u.teams).ThenInclude(t => t.certificates).ThenInclude(cert => cert.password)
											.FirstOrDefault(u => u.Id == userid.ToString());
                if( user != null)
                {
					allCertificates = user.teams.Where(t => t.client.id == clientid && t.certificates.Count > 0).Select(t => new TeamCertificatesMap() { teamid= t.id, clientid = t.client.id, teamname =t.name, certificates = t.certificates }).ToList();

                }
				


				return certificateDataMapper.ConvertCertificateListToCertificateDTOList(allCertificates);
			}
            return new List<CertificateDTO>();
		}

        public SetStatus UploadCertificate(IConfiguration configuration, PSDBContext _dbContext, UserAuthorization _userAuthorization, SymmetricEncryption _symmetricEncryption, UploadCertificatecs uploadCertificate, Guid _userid, [FromServices] ILogger<UserAuthorization> _logger)
        {
			_userAuthorization.IsCertificateUploadAuthorization(uploadCertificate.team, _dbContext, _userid, out List<ClientTeamPair> writeAuthorization, _logger);
			if(writeAuthorization.Count > 0)
			{
				List<IFormFile> files = new List<IFormFile>();
				files.Add(uploadCertificate.certfile);
				if(uploadCertificate.certpass == null && uploadCertificate.certkey != null)
				{
					files.Add(uploadCertificate.certkey);
					foreach (ClientTeamPair pair in writeAuthorization)
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

						if (IsValidCertificateName(files))
						{
							if (TryPemCertificateValidation(configuration, uploadCertificate.certfile, uploadCertificate.certkey, certfile.currentFileName, keyfile.currentFileName, out X509Certificate2 validCertificate))
							{
								Certificate newCertificate = new Certificate();
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


								Client client = _dbContext.Clients.Include(c => c.teams).FirstOrDefault(c => c.id == pair.clientid);
								if (client != null)
								{
									Team team = client.teams.FirstOrDefault(t => t.id == pair.teamid);
									if (team != null)
									{
										if (team.credentials != null)
										{
											team.certificates.Add(newCertificate);
										}
										else
										{
											team.certificates = [newCertificate];
										}
										_dbContext.Teams.Update(team);
										_dbContext.CertificatesFile.Add(certfile);
										_dbContext.CertificatesFile.Add(keyfile);
										_dbContext.Certificates.Add(newCertificate);
										_dbContext.SaveChanges();
									}
									else
									{
										_logger.LogInformation($"{DateTime.Now} - Invalid team to upload certificate");
										return new SetStatus() { status = "KO" };
									}
								}
								else
								{
									_logger.LogInformation($"{DateTime.Now} - Invalid client to upload certificate");
									return new SetStatus() { status = "KO" };
								}

							}
							else
							{
								_logger.LogInformation($"{DateTime.Now} - Invalid construction of certificate using PEM files\n");
								return new SetStatus() { status = "KO", errorMessage = "Invalid Certificate" };
							}
						}
						else
						{
							_logger.LogInformation($"{DateTime.Now} - Invalid Certificate file name\n");
							_logger.LogDebug($"{DateTime.Now} - Invalid Certificate file names:  {string.Join(',',files.Select(f => f.Name).ToList())}\n");
							return new SetStatus() { status = "KO",errorMessage="Invalid Certificate file name" };
						}
					}
					return new SetStatus() { status = "OK" };
				}
				else if(uploadCertificate.certpass != null && uploadCertificate.certkey == null)
				{
					foreach (ClientTeamPair pair in writeAuthorization)
					{
						CertificateFile certfile = new CertificateFile();
						certfile.id = Guid.NewGuid();
						certfile.currentFileName = certfile.id.ToString() + "." + uploadCertificate.certfile.FileName.Split(".")[1];
						certfile.originalFileName = uploadCertificate.certfile.FileName;
						certfile.createdate = DateTime.Now;


						if (IsValidCertificateName(files))
						{

							if (TryCertificateValidation(configuration, uploadCertificate.certfile, certfile.currentFileName, uploadCertificate.certpass, out X509Certificate2 validCertificate))
							{
								Certificate newCertificate = new Certificate();
								newCertificate.id = Guid.NewGuid();
								newCertificate.name = validCertificate.FriendlyName;
								newCertificate.issuedBy = ConvertDNtoCN(validCertificate.Issuer);
								newCertificate.issuedTo = ConvertDNtoCN(validCertificate.Subject);
								newCertificate.friendlyname = validCertificate.FriendlyName;
								newCertificate.expirationDate = validCertificate.NotAfter;
								newCertificate.file = certfile;
								newCertificate.createdate = DateTime.Now;
								newCertificate.updatedate = DateTime.Now;
								SymmetricKey key =_symmetricEncryption.EncryptString(uploadCertificate.certpass, configuration);

								newCertificate.password = new Password() { aad = key.aad, password = key.password,createdate = DateTime.Now, updatedate = DateTime.Now, id = Guid.NewGuid()};

								Client client = _dbContext.Clients.Include(c => c.teams).FirstOrDefault( c => c.id == pair.clientid);
								if(client != null)
								{
									Team team = client.teams.FirstOrDefault(t => t.id == pair.teamid);
									if(team != null)
									{
										if(team.credentials != null)
										{
											team.certificates.Add(newCertificate);
										}
										else
										{
											team.certificates = [newCertificate];
										}
										_dbContext.Teams.Update(team);
										_dbContext.Passwords.Add(newCertificate.password);
										_dbContext.CertificatesFile.Add(certfile);
										_dbContext.Certificates.Add(newCertificate);
										_dbContext.SaveChanges();
										
									}
								}


							}
							else
							{
								_logger.LogInformation($"{DateTime.Now} - Invalid construction of certificate using cert file or cert password\n");
								return new SetStatus() { status = "KO", errorMessage = "Invalid Certificate file name" };
							}
							
						}
						else
						{
							_logger.LogInformation($"{DateTime.Now} - Invalid Certificate file name\n");
							_logger.LogDebug($"{DateTime.Now} - Invalid Certificate file names:  {string.Join(',', files.Select(f => f.Name).ToList())}\n");
							return new SetStatus() { status = "KO", errorMessage = "Invalid Certificate file name" };
						}
						
					}
					return new SetStatus() { status = "OK" };
				}
				else
				{
					_logger.LogCritical($"{DateTime.Now} - Invalid upload for constructing certificate\n");
					return new SetStatus() { status = "KO", errorMessage = "Invalid upload for constructing certificate" };
				}
			}
			{
				_logger.LogCritical($"{DateTime.Now} - Permissions denied for upload certificates\n");
				return new SetStatus() { status = "KO", errorMessage = "Permissions denied for upload certificates" };
			}
			
        }

		public bool IsValidCertificateName(List<IFormFile> files)
		{
			Regex regexname = new Regex(@"^[a-zA-Z0-9_-]*$");
			List<string> privatekeypostfix = ["pfx", "p12", "pem", "crt", "key"];
			return files.All(file => regexname.IsMatch(file.FileName.Split(".")[0]) && privatekeypostfix.Any(postfix => postfix == file.FileName.Split(".")[1]));
		}

		public bool ifItemExist(List<string> strings, string item)
        {
            if (strings.FirstOrDefault(s => s == item) != null)
            {
                return true;
            }
            return false;
        }
    
        public bool TrySaveCertificate(IConfiguration configuration, IFormFile cert, string certname)
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

		public bool TrySaveCertificate(IConfiguration configuration, IFormFile cert, IFormFile key, string certname, string keyname)
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
    
		public bool AddCertificateToTeams(List<ClientTeamPair> teams, PSDBContext _dbContext, Certificate cert, CertificateFile certfile)
		{

			try
			{
				foreach (ClientTeamPair team in teams)
				{
					Certificate tempCert = cert;
					CertificateFile tempcertfile = certfile;
					Team editedTeam = _dbContext.Clients.Include(c => c.teams).FirstOrDefault(c => c.id == team.clientid).teams.FirstOrDefault(t => t.id == team.teamid);
					tempCert.id = Guid.NewGuid();
					tempcertfile.id = Guid.NewGuid();
					tempCert.file = tempcertfile;
					if (editedTeam.certificates != null)
					{
						editedTeam.certificates.Add(cert);
					}
					else
					{
						editedTeam.certificates = [cert];
					}
					_dbContext.Update(editedTeam);
					_dbContext.SaveChanges();
				}
				
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public bool AddCertificateToTeams(List<ClientTeamPair> teams, PSDBContext _dbContext, Certificate cert, CertificateFile certfile, CertificateFile keyfile)
		{

			try
			{
				foreach (ClientTeamPair team in teams)
				{
					Certificate tempCert = cert;
					CertificateFile tempcertfile = certfile;
					CertificateFile tempkeyfile = keyfile;
					Team editedTeam = _dbContext.Clients.Include(c => c.teams).FirstOrDefault(c => c.id == team.clientid).teams.FirstOrDefault(t => t.id == team.teamid);
					tempCert.id = Guid.NewGuid();
					tempcertfile.id = Guid.NewGuid();
					tempkeyfile.id = Guid.NewGuid();
					tempCert.file = tempcertfile;
					tempCert.key = tempkeyfile;
					if (editedTeam.certificates != null)
					{
						editedTeam.certificates.Add(cert);
					}
					else
					{
						editedTeam.certificates = [cert];
					}
					_dbContext.Update(editedTeam);
					_dbContext.SaveChanges();
				}

				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
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
	
		public SetStatus Delete(DeleteItem deleteItem, PSDBContext _dbContext, IConfiguration _configuration, JwtTokenManager _jwtTokenManager, string jwt)
		{
			if(_jwtTokenManager.GetUserID(jwt,out Guid _userid))
			{
				User user = _dbContext.Users.Include(u => u.teams)
											.Include(u => u.teams).ThenInclude(t => t.certificates)
											.FirstOrDefault(u => u.Id == _userid.ToString() && u.teams.Any(t => t.id == deleteItem.teamid && t.certificates.Any(c => c.id == deleteItem.id)));
				if(user != null)
				{
					Certificate removeCertificate = _dbContext.Certificates.Include(c => c.password)
																			.Include(c => c.file)
																			.Include(c => c.key)
																			.FirstOrDefault(c => c.id == deleteItem.id);


					DeleteCertificateFile(_configuration, removeCertificate.file.currentFileName);
					if(removeCertificate.key != null)
					{
						DeleteCertificateFile(_configuration, removeCertificate.key.currentFileName);
						_dbContext.CertificatesFile.Remove(removeCertificate.key);

					}
					if(removeCertificate.password != null)
					{
						_dbContext.Passwords.Remove(removeCertificate.password);
					}
					
					user.teams.FirstOrDefault(t => t.id == deleteItem.teamid).certificates.Remove(removeCertificate);
					_dbContext.CertificatesFile.Remove(removeCertificate.file);
					_dbContext.Certificates.Remove(removeCertificate);
					_dbContext.Teams.Update(user.teams.FirstOrDefault(t => t.id == deleteItem.teamid));
					_dbContext.SaveChanges();

					return new SetStatus() { status = "OK" };
				}
				return new SetStatus() { status = "KO" };
			}
			return new SetStatus() { status = "KO" };
		}
	
	
		public ResponseDownloadCertificate DownloadCertificate([FromServices] PSDBContext _dbContext, [FromServices] IConfiguration _configuration, [FromServices] JwtTokenManager _jwtTokenManager, string jwt , RequestDownloadCertificate downloadCertificate)
		{
			if(_jwtTokenManager.GetUserID(jwt,out Guid _userid))
			{
				User user = _dbContext.Users.Include(u => u.teams)
											.Include(u => u.teams).ThenInclude(t => t.certificates)
											.Include(u => u.teams).ThenInclude(t => t.certificates).ThenInclude(c => c.file)
											.FirstOrDefault(u => u.Id == _userid.ToString() && u.teams.Any(t => t.id == downloadCertificate.teamId && t.certificates.Any(c => c.id == downloadCertificate.certificateId)));
				if(user != null)
				{
					Certificate certificate = user.teams.FirstOrDefault(t => t.id == downloadCertificate.teamId)
									  .certificates.FirstOrDefault(c => c.id == downloadCertificate.certificateId);
					if(certificate != null)
					{
						if(certificate.key == null)
						{
							ResponseDownloadCertificate output = new ResponseDownloadCertificate();
							output.FileContent = new FileContentResult(System.IO.File.ReadAllBytes(_configuration.GetSection("CertificateLocation").Value + "\\" + certificate.file.currentFileName), "application/octet-stream");
							output.filename = certificate.file.originalFileName;
							return output;
						}
							return null;
					}
					return null;

				}
				return null;
			}
			return null;

		}

		public ResponseDownloadCertificate DownloadCertificateKey([FromServices] PSDBContext _dbContext, [FromServices] IConfiguration _configuration, [FromServices] JwtTokenManager _jwtTokenManager, string jwt, RequestDownloadCertificate downloadCertificate)
		{
			if (_jwtTokenManager.GetUserID(jwt, out Guid _userid))
			{
				User user = _dbContext.Users.Include(u => u.teams)
											.Include(u => u.teams).ThenInclude(t => t.certificates)
											.Include(u => u.teams).ThenInclude(t => t.certificates).ThenInclude(c => c.key)
											.FirstOrDefault(u => u.Id == _userid.ToString() && u.teams.Any(t => t.id == downloadCertificate.teamId && t.certificates.Any(c => c.id == downloadCertificate.certificateId)));
				if (user != null)
				{
					Certificate certificate = user.teams.FirstOrDefault(t => t.id == downloadCertificate.teamId)
									  .certificates.FirstOrDefault(c => c.id == downloadCertificate.certificateId);
					if (certificate != null)
					{
						if (certificate.key != null)
						{
							ResponseDownloadCertificate output = new ResponseDownloadCertificate();
							output.FileContent = new FileContentResult(System.IO.File.ReadAllBytes(_configuration.GetSection("CertificateLocation").Value + "\\" + certificate.key.currentFileName), "application/octet-stream");
							output.filename = certificate.key.originalFileName;
							return output;
						}
					}
					else
					{
						return null;
					}
				}
				else
				{
					return null;
				}
			}
			return null;

		}

	}
}
