using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel.ClientDTO
{
    public class ClientDTOForAdmins
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public DateTime createdate { get; set; }
        public DateTime updatedate { get; set; }
    }
}
