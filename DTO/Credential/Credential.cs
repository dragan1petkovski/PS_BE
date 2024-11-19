namespace DTO.Credential
{
    public class Credential
    {
        public Guid id {  get; set; }
        public string? domain { get; set; }
        public string username {  get; set; }
        public string? email { get; set; }
        public string? remote { get; set; }
        public string password { get; set; }

        public string? teamname { get; set; }
        public Guid clientid { get; set; }
        public Guid teamid { get; set; }
        public string? note { get; set; }
    }
}
