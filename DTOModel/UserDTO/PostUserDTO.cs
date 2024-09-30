using DTOModel.TeamDTO;
using System.ComponentModel.DataAnnotations;

namespace DTOModel.UserDTO
{
    public class PostUserDTO // "Add User" data
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_-]*$")]
        public string firstname {  get; set; }

		[Required]
		[RegularExpression(@"^[a-zA-Z0-9_-]*$")]
		public string lastname { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

		[Required]
		[RegularExpression(@"^[a-zA-Z0-9]*$")]
		public string username { get; set; }

        public List<ClientTeamPair> clientTeamPairs { get; set; }
    }
}
