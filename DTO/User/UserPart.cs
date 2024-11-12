using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.User
{
    public class UserPart //data for "Add Team" table
    {
        public Guid id {  get; set; }
        public string username { get; set; }
        public string fullname { get; set; }
    }
}
