using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel.UserDTO
{
    public class UserPartDTO
    {
        public Guid id {  get; set; }
        public string username { get; set; }
        public string fullname { get; set; }
    }
}
