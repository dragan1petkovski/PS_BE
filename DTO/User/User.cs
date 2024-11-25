using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.User
{
    public class User // For Admin table
    {
        public Guid id {  get; set; }
        public string? rolename { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public DateTime createdate { get; set; }
        public DateTime updatedate { get; set; }
    }
}
