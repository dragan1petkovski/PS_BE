using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel.ClientDTO
{
    public class PostClient
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]*$")]
        public string name {  get; set; }
    }
}
