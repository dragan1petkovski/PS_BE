using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DomainModel
{
    public class Credential
    {
        [Key] public Guid id { get; set; }
        public string domain { get; set; }
        public string username { get; set; }
        public string? email { get; set; }
        public string? remote { get; set; }

        public string? note { get; set; }

        public DateTime createdate { get; set; }
        public DateTime updatedate { get; set; }

        public Password password { get; set; }
    }
}
