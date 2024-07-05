using Microsoft.Extensions.Configuration;
using DTOModel;

namespace Services
{
    internal class CertificateService
    {
        private readonly IConfiguration _configuration;
        public CertificateService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<CertificateDTO> GetCertificatesByUserIdClintId(Guid clientid, Guid userid)
        {
            return new List<CertificateDTO>();
        }
    }
}
