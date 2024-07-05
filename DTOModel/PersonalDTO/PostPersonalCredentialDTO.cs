using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel.PersonalDTO
{
    public class PostPersonalCredentialDTO
    {
        public string domain {  get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string remote {  get; set; }
        public string email { get; set; }
        public string note { get; set; }

        public Guid personalFolderId { get; set; }
    }
}
