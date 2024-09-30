using DomainModel;

namespace TransitionObjectMapper
{
    public class TeamCertificatesMap
    {
        public string teamname {  get; set; }

        public Guid clientid { get; set; }
        public Guid teamid { get; set; }
        public List<Certificate> certificates { get; set; }
    }
}
