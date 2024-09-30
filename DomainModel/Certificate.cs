using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DomainModel
{
    public class Certificate
    {

        [Key] public Guid id { get; set; }

        public string name { get; set; }

        public string friendlyname { get; set; }

        public string issuedBy { get; set; }

        public string issuedTo { get; set; }

        public DateTime expirationDate { get; set; }

        public DateTime createdate { get; set; }
        public DateTime updatedate { get; set; }

        public Password? password { get; set; }

        public CertificateFile file { get; set; }
        public CertificateFile? key {  get; set; }

	}
}
