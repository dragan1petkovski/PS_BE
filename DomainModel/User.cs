using Microsoft.AspNetCore.Identity;

namespace DomainModel
{
    public class User: IdentityUser
    {
        public string firstname { get; set; }

        public string lastname { get; set; }

        public DateTime createdate { get; set; }
        public DateTime updatedate { get; set; }

        public List<PersonalFolder> folders { get; set; }
        public List<Team> teams { get; set; }
        public List<Credential> credentials { get; set; }


        public string Fullname => firstname + " " + lastname;
    }
}
