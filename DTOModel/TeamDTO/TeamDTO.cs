using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel.TeamDTO
{
    public class TeamDTO
    {
        public Guid id { get; set; }
        public string clientname { get; set; }
        public string name { get; set; }
        public DateTime createdate { get; set; }
        public DateTime updatedate { get; set; }
    }
}
