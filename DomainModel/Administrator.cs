using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel
{
    public class Administrator
    {
        [Key] public Guid id { get; set; }
        public string firstname { get; set; }

        public string lastname { get; set; }

        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string createdate { get; set; }
        public string updatedate { get; set; }

        public List<User> users { get; set; }
        public List<Client> clients { get; set; }
        public List<Team> teams { get; set; }

    }
}
