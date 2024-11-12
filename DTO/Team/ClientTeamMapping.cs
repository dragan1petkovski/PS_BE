using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Team
{
    public class ClientTeamMapping
    {
        public Guid teamid { get; set; }
        public Guid clientid { get; set; }
        public string clientname { get; set; }
        public string teamname { get; set; }


        public override string ToString()
        {
            return clientname + " : "+clientid + " - "+teamname + " : "+ teamid;
        }
    }
}
