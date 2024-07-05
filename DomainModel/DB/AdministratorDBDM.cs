using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.DB
{
    public class AdministratorDBDM
    {
        [Key] public Guid id { get; set; }
        public string firstname { get; set; }

        public string lastname { get; set; }

        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string createdate { get; set; }
        public string updatedate { get; set; }
        
        public List<UserDBDM> users { get; set; }
        public List<ClientDBDM> clients { get; set; }
        public List<TeamDBDM> teams { get; set; }

    }
}
