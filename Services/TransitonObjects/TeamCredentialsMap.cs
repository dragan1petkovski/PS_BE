using DomainModel.DB;

namespace DTOModel.CredentialDTO
{
    public class TeamCredentialsMap
    {
        public string teamname {  get; set; }

        public Guid teamid { get; set; }
        public List<CredentialDBDM> credentials { get; set; }
    }
}
