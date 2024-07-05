using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DomainModel.DB
{
    public class TeamDBDM
    {
        [Key] public Guid id { get; set; }

        public string name { get; set; }
        
        public DateTime createdate { get; set; }
        public DateTime updatedate { get; set; }

        public List<CertificateDBDM> certificates { get; set; }
        public List<CredentialDBDM> credentials { get; set; }

        public ClientDBDM client { get; set; }
        public List<UserDBDM> users { get; set; }
    }
}
