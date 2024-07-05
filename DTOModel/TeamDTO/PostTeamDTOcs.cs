using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel.TeamDTO
{
    public class PostTeamDTOcs
    {
        public string name {  get; set; }
        public Guid clientid { get; set; }

        public List<Guid> userids { get; set; }
    }
}
