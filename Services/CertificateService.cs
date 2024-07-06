using DTOModel;
using DataAccessLayerDB;
using Microsoft.EntityFrameworkCore;
using DomainModel.DB;

namespace Services
{
    internal class CertificateService
    {
        private readonly PSDBContext _dbContext;
        public CertificateService(PSDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<CertificateDTO> GetCertificatesByUserIdClintId( Guid userid, Guid clientid)
        {
            List<CertificateDBDM> allCertificates = new List<CertificateDBDM>();
            UserDBDM user = _dbContext.Users.Include(u => u.teams)
                            .Include(u => u.teams).ThenInclude(t => t.client)
                            .Include(u => u.teams).ThenInclude(t => t.certificates)
                            .Include(u => u.teams).ThenInclude(t => t.certificates).ThenInclude(cert => cert.password)
                            .First(u => u.id ==userid);
            user.teams.Where(t => t.client.id == clientid && t.certificates.Count > 0).Select(t => t.certificates).ToList().ForEach(certList => allCertificates.AddRange(certList));
            
            
            return ConvertCertificateDBDMListToCertificateDTOList(allCertificates);
        }

        private List<CertificateDTO> ConvertCertificateDBDMListToCertificateDTOList(List<CertificateDBDM> certificates)
        {
            List<CertificateDTO> result = new List<CertificateDTO> ();

            foreach(CertificateDBDM cert in certificates)
            {
                result.Add(new CertificateDTO()
                {
                    name = cert.name,
                    friendlyname = cert.friendlyname,
                    issuedby = cert.issuedBy,
                    issuedto = cert.issuedTo,
                    expirationdate = cert.expirationDate,
                    id = cert.id,
                    passwordid = cert.password.id,
                    teamname = ""
                });
            }

            return result;
        }
    }
}
