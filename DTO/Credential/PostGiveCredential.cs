namespace DTO.Credential
{
    public class PostGiveCredential
    {
        public string domain {  get; set; }
        public string username {  get; set; }
        public string password { get; set; }
        public string email {  get; set; }
        public string remote {  get; set; }
        public string note { get; set; }
        public List<Guid> userids { get; set; }
        public List<Guid> teamids { get; set; }
    }
}
