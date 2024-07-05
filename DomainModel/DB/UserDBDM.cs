using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainModel.DB
{
    public class UserDBDM
    {
        [Key] public Guid id { get; set; }
        public string firstname { get; set; }

        public string lastname { get; set; }

        public string username { get; set; }
        public string email { get; set; } 
        public string password { get; set; }

        public DateTime createdate { get; set; }
        public DateTime updatedate { get; set; }

        public List<PersonalFolderDBDM> folders { get; set; }
        public List<ClientDBDM> clients { get; set; }
        public List<TeamDBDM> teams { get; set; }
        public List<CredentialDBDM> credentials { get; set; }
        public List<CertificateDBDM> certificates { get; set; }


        public string Fullname => this.firstname + " " + this.lastname;
    }
}
