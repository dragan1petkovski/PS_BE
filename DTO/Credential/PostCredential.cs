using System.ComponentModel.DataAnnotations;
using DTO.Team;

namespace DTO.Credential
{
    public class PostCredential
    {
        [RegularExpression(@"^[A-Za-z0-9]*$")]
        [Required]
        public string domain {  get; set; }

        public string? email { get; set; }
        public string? note { get; set; }

		[Required]
		public string password { get; set; }
        public string? remote {  get; set; }
        public List<ClientTeamPair> teams { get; set; }

		[Required]
		[RegularExpression(@"^[A-Za-z0-9_-]*$")]
		public string username { get; set; }
    }
}
