using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainModel.DB
{
    public class ClientDBDM
    {
        //CLient must have complex primary key that combines ID and Name as primary key.
        //We don't want to end up having two clients with same name
        [Key] public Guid id { get; set; }
        public string name { get; set; }

        public DateTime createdate { get; set; }
        public DateTime updatedate { get; set; }

        public List<UserDBDM> users { get; set; }
        public List<TeamDBDM> teams { get; set; }
    }
}
