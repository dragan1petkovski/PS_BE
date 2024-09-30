using DTOModel;
using DomainModel;
using TransitionObjectMapper;
using Microsoft.AspNetCore.Http;

namespace DataMapper
{
	public class CertificateDataMapper
	{
		public List<CertificateDTO> ConvertCertificateListToCertificateDTOList(List<TeamCertificatesMap> certificates)
		{
			List<CertificateDTO> result = new List<CertificateDTO>();

			foreach (TeamCertificatesMap certs in certificates)
			{
				foreach(Certificate cert in certs.certificates)
				{
					if(cert.password == null)
					{
						result.Add(new CertificateDTO()
						{
							name = cert.name,
							friendlyname = cert.friendlyname,
							issuedby = cert.issuedBy,
							issuedto = cert.issuedTo,
							expirationdate = cert.expirationDate,
							id = cert.id,
							teamname = certs.teamname,
							clientid = certs.clientid,
							teamid = certs.teamid,
							pem = true

						});
					}
					else
					{
						Console.WriteLine(cert.password.id);
						result.Add(new CertificateDTO()
						{
							name = cert.name,
							friendlyname = cert.friendlyname,
							issuedby = cert.issuedBy,
							issuedto = cert.issuedTo,
							expirationdate = cert.expirationDate,
							id = cert.id,
							teamname = certs.teamname,
							clientid = certs.clientid,
							teamid = certs.teamid,
							pem = false

						});
					}

				}

			}

			return result;
		}
	}
}
