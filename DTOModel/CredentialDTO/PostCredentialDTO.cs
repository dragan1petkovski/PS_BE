using DTOModel.UserDTO;

namespace DTOModel.CredentialDTO
{
    public class PostCredentialDTO
    {
        public string domain {  get; set; }
        public string? email { get; set; }
        public string? note { get; set; }
        public string password { get; set; }
        public string? remote {  get; set; }
        public List<ClientTeamPair> teams { get; set; }
        public string username { get; set; }
    }
}
