using DTOModel.TeamDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel.CredentialDTO
{
    public class PostCredentialDTO
    {
        public string domain {  get; set; }
        public string email { get; set; }
        public string note { get; set; }
        public string password { get; set; }
        public string remote {  get; set; }
        public List<ClientTeamMapping> teams { get; set; }
        public string username { get; set; }
    }
}
