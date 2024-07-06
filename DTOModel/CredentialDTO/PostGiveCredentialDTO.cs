using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel.CredentialDTO
{
    public class PostGiveCredentialDTO
    {
        public string domain {  get; set; }
        public string username {  get; set; }
        public string password { get; set; }
        public string email {  get; set; }
        public string remote {  get; set; }
        public string note { get; set; }
        public List<Guid> userids { get; set; }
        public List<Guid> teamids { get; set; }
    }
}
