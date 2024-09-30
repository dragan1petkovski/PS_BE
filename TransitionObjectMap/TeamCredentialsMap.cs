using DomainModel;

namespace TransitionObjectMapper
{
    public class TeamCredentialsMap
    {
        public string teamname {  get; set; }

        public Guid clientid { get; set; }
        public Guid teamid { get; set; }
        public List<Credential> credentials { get; set; }
    }
}
