using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel.UserDTO
{
    public class ClientTeamPair
    {
        public Guid teamid {  get; set; }
        public Guid clientid { get; set; }
    }
}
