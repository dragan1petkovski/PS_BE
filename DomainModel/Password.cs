using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DomainModel
{
    public class Password
    {
        [Key] public Guid id { get; set; }
        public string password { get; set; }

        public string aad {  get; set; }
        public DateTime createdate { get; set; }
        public DateTime updatedate { get; set; }
    }
}
