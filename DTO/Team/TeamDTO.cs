using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Team
{
    public class Team
    {
        public Guid id { get; set; }
        public Guid clientid { get; set; }
        public string clientname { get; set; }
        public string name { get; set; }
        public DateTime createdate { get; set; }
        public DateTime updatedate { get; set; }
    }
}
