using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Certificate
{
    public class Certificate
    {
        public Guid id {  get; set; }
        public string name { get; set; }
        public string friendlyname { get; set; }
        public string issuedby { get; set; }
        public string issuedto { get; set; }
        public DateTime expirationdate { get; set; }
        public string teamname { get; set; }

        public bool pem {  get; set; }
        public Guid teamid {  get; set; }
        public Guid clientid { get; set; }
    }
}
