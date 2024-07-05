using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DomainModel.DB
{
    public class PasswordDBDM
    {
        [Key] public Guid id { get; set; }
        public string password { get; set; }

        public DateTime createdate { get; set; }
        public DateTime updatedate { get; set; }
    }
}
