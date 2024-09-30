using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel.PersonalDTO
{
    public class PostPersonalFolderDTO
    {
        [RegularExpression(@"^[a-zA-Z0-9_-]*$")]
        [Required]
        public string name {  get; set; }
    }
}
