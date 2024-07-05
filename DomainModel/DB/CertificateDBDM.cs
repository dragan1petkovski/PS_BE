using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DomainModel.DB
{
    public class CertificateDBDM
    {

        [Key] public Guid id { get; set; }

        public string name { get; set; }

        public string friendlyname { get; set; }

        public string issuedBy { get; set; }

        public string issuedTo { get; set; }

        public DateTime expirationDate { get; set; }

        public DateTime createdate { get; set; }
        public DateTime updatedate { get; set; }

        public PasswordDBDM password { get; set; }

    }
}
