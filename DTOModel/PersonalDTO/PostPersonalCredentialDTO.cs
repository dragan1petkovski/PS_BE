using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel.PersonalDTO
{
    public class PostPersonalCredentialDTO
    {
        [Required]
		[RegularExpression(@"^[a-zA-Z0-9_-]*$")]
		public string domain {  get; set; }

		[Required]
		[RegularExpression(@"^[a-zA-Z0-9_-]*$")]
		public string username { get; set; }

		[Required]
		public string password { get; set; }
        public string? remote {  get; set; }

		public string? email { get; set; }

        public string? note { get; set; }

        public Guid? personalFolderId { get; set; }
    }
}
