using DTO.Certificate;
using DomainModel;
using TransitionObjectMapper;
using Microsoft.AspNetCore.Http;

namespace DataMapper
{
	public class CertificateDataMapper
	{
		public List<DTO.Certificate.Certificate> ConvertCertificateListToCertificateDTOList(List<TeamCertificatesMap> certificates)
		{
			List<DTO.Certificate.Certificate> result = new List<DTO.Certificate.Certificate>();

			foreach (TeamCertificatesMap certs in certificates)
			{
				foreach(DomainModel.Certificate cert in certs.certificates)
				{
					if(cert.password == null)
					{
						result.Add(new DTO.Certificate.Certificate()
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
						result.Add(new DTO.Certificate.Certificate()
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
