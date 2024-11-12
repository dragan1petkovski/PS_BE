namespace DTO.Credential
{
    public class PersonalCredential
    {
        public Guid id {  get; set; }
        public string? domain { get; set; }
        public string username {  get; set; }
        public string? email { get; set; }
        public string? remote { get; set; }
        public string password { get; set; }

        public Guid? personalfolderid { get; set; }
        public string? note { get; set; }
    }
}
