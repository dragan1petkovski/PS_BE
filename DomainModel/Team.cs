using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DomainModel
{
    public class Team
    {
        [Key] public Guid id { get; set; }

        public string name { get; set; }

        public DateTime createdate { get; set; }
        public DateTime updatedate { get; set; }

        public List<Certificate> certificates { get; set; }
        public List<Credential> credentials { get; set; }

        public Client client { get; set; }
        public List<User> users { get; set; }
    }
}
