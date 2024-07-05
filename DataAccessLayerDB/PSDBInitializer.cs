using DomainModel.DB;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;

namespace DataAccessLayerDB
{
    public class PSDBInitializer
    {
        private readonly PSDBContext _context;

        public PSDBInitializer(PSDBContext context)
        {
            _context = context;
        }

        public void Run()
        {
            ClientDBDM c1 = new ClientDBDM { id = Guid.NewGuid(), name = "Client1", createdate = DateTime.Now, updatedate = DateTime.Now };
            ClientDBDM c2 = new ClientDBDM { id = Guid.NewGuid(), name = "Client2", createdate = DateTime.Now, updatedate = DateTime.Now };
            ClientDBDM c3 = new ClientDBDM { id = Guid.NewGuid(), name = "Client3", createdate = DateTime.Now, updatedate = DateTime.Now };
            ClientDBDM c4 = new ClientDBDM { id = Guid.NewGuid(), name = "Client4", createdate = DateTime.Now, updatedate = DateTime.Now };
            ClientDBDM c5 = new ClientDBDM { id = Guid.NewGuid(), name = "Client5", createdate = DateTime.Now, updatedate = DateTime.Now };

            TeamDBDM t1 = new TeamDBDM { id = Guid.NewGuid(), name = "Team1", createdate = DateTime.Now, updatedate = DateTime.Now };
            TeamDBDM t2 = new TeamDBDM { id = Guid.NewGuid(), name = "Team2", createdate = DateTime.Now, updatedate = DateTime.Now };
            TeamDBDM t3 = new TeamDBDM { id = Guid.NewGuid(), name = "Team3", createdate = DateTime.Now, updatedate = DateTime.Now };
            TeamDBDM t4 = new TeamDBDM { id = Guid.NewGuid(), name = "Team4", createdate = DateTime.Now, updatedate = DateTime.Now };
            TeamDBDM t5 = new TeamDBDM { id = Guid.NewGuid(), name = "Team5", createdate = DateTime.Now, updatedate = DateTime.Now };
            TeamDBDM t6 = new TeamDBDM { id = Guid.NewGuid(), name = "Team6", createdate = DateTime.Now, updatedate = DateTime.Now };
            TeamDBDM t7 = new TeamDBDM { id = Guid.NewGuid(), name = "Team7", createdate = DateTime.Now, updatedate = DateTime.Now };
            TeamDBDM t8 = new TeamDBDM { id = Guid.NewGuid(), name = "Team8", createdate = DateTime.Now, updatedate = DateTime.Now };
            TeamDBDM t9 = new TeamDBDM { id = Guid.NewGuid(), name = "Team9", createdate = DateTime.Now, updatedate = DateTime.Now };
            TeamDBDM t10 = new TeamDBDM { id = Guid.NewGuid(), name = "Team10", createdate = DateTime.Now, updatedate = DateTime.Now };
            TeamDBDM t11 = new TeamDBDM { id = Guid.NewGuid(), name = "Team1", createdate = DateTime.Now, updatedate = DateTime.Now };
            TeamDBDM t12 = new TeamDBDM { id = Guid.NewGuid(), name = "Team2", createdate = DateTime.Now, updatedate = DateTime.Now };
            TeamDBDM t13 = new TeamDBDM { id = Guid.NewGuid(), name = "Team3", createdate = DateTime.Now, updatedate = DateTime.Now };
            TeamDBDM t14 = new TeamDBDM { id = Guid.NewGuid(), name = "Team4", createdate = DateTime.Now, updatedate = DateTime.Now };
            TeamDBDM t15 = new TeamDBDM { id = Guid.NewGuid(), name = "Team5", createdate = DateTime.Now, updatedate = DateTime.Now };

            UserDBDM u1 = new UserDBDM { id = Guid.NewGuid(), firstname = "user1", lastname = "userski1", username="username1", createdate = DateTime.Now, updatedate = DateTime.Now, email = "user1userski1@test.com", password = "test123" };
            UserDBDM u2 = new UserDBDM { id = Guid.NewGuid(), firstname = "user2", lastname = "userski2", username="username2", createdate = DateTime.Now, updatedate = DateTime.Now, email = "user2userski2@test.com", password = "test123" };
            UserDBDM u3 = new UserDBDM { id = Guid.NewGuid(), firstname = "user3", lastname = "userski3", username="username3", createdate = DateTime.Now, updatedate = DateTime.Now, email = "user3userski3@test.com", password = "test123" };
            UserDBDM u4 = new UserDBDM { id = Guid.NewGuid(), firstname = "user4", lastname = "userski4", username="username4", createdate = DateTime.Now, updatedate = DateTime.Now, email = "user4userski4@test.com", password = "test123" };
            UserDBDM u5 = new UserDBDM { id = Guid.NewGuid(), firstname = "user5", lastname = "userski5", username="username5", createdate = DateTime.Now, updatedate = DateTime.Now, email = "user5userski5@test.com", password = "test123" };


            c1.teams = [t1, t2,t13];
            c2.teams = [t3, t4,t11, t5];
            c3.teams = [t6,t12, t7];
            c4.teams = [t9,t14, t8];
            c5.teams = [t10,t15];

            u1.teams = [t10, t2, t3, t4, t6, t7];
            u2.teams = [t3, t4, t6];
            u3.teams = [t6];
            u4.teams = [t7, t8, t10];

            u1.clients = [c5,c1,c2,c3];
            u2.clients = [c2,c3];
            u3.clients = [c3];
            u4.clients = [c3,c4,c5];

            PasswordDBDM p1 = new PasswordDBDM { id = Guid.NewGuid(), password = " |7{25^{,Qh", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p2 = new PasswordDBDM { id = Guid.NewGuid(), password = " &+}ELFw7OX", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p3 = new PasswordDBDM { id = Guid.NewGuid(), password = " *6_),pS.92", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p4 = new PasswordDBDM { id = Guid.NewGuid(), password = " <%D;]Da]r", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p5 = new PasswordDBDM { id = Guid.NewGuid(), password = " laOp3sl59X", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p6 = new PasswordDBDM { id = Guid.NewGuid(), password = " +Ua|bHF>{", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p7 = new PasswordDBDM { id = Guid.NewGuid(), password = " 9Rj.&rN*", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p8 = new PasswordDBDM { id = Guid.NewGuid(), password = " +8<z6hhK@<", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p9 = new PasswordDBDM { id = Guid.NewGuid(), password = " S)40c5Vf66", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p10 = new PasswordDBDM { id = Guid.NewGuid(), password = " gwr)_<Z?d", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p11 = new PasswordDBDM { id = Guid.NewGuid(), password = " gsMkJKqxlo", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p12 = new PasswordDBDM { id = Guid.NewGuid(), password = " =_O]!4_zAI", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p13 = new PasswordDBDM { id = Guid.NewGuid(), password = " Ku2NM-K602", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p14 = new PasswordDBDM { id = Guid.NewGuid(), password = " fP(iWME.sV", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p15 = new PasswordDBDM { id = Guid.NewGuid(), password = " PU((c,,|vs", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p16 = new PasswordDBDM { id = Guid.NewGuid(), password = " 4;Rrb3O8<Q", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p17 = new PasswordDBDM { id = Guid.NewGuid(), password = " Mxcuue4lM)", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p18 = new PasswordDBDM { id = Guid.NewGuid(), password = " M6]jlAM=Ab", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p19 = new PasswordDBDM { id = Guid.NewGuid(), password = " sg*m4z:F99", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p20 = new PasswordDBDM { id = Guid.NewGuid(), password = " |,Y3V**D}w", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p21 = new PasswordDBDM { id = Guid.NewGuid(), password = " e^rQu_Ed+2", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p22 = new PasswordDBDM { id = Guid.NewGuid(), password = " 443,+E>}kR", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p23 = new PasswordDBDM { id = Guid.NewGuid(), password = " zF]Dhpog2M", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p24 = new PasswordDBDM { id = Guid.NewGuid(), password = " .3^LjZ5f|[", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p25 = new PasswordDBDM { id = Guid.NewGuid(), password = " r-;Ph[OEP#", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p26 = new PasswordDBDM { id = Guid.NewGuid(), password = " yz<@p-l]H", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p27 = new PasswordDBDM { id = Guid.NewGuid(), password = " Qy||tx1OH-", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p28 = new PasswordDBDM { id = Guid.NewGuid(), password = " +Ov%;:IfHp", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p29 = new PasswordDBDM { id = Guid.NewGuid(), password = " 6PkJ5#0iAc", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p30 = new PasswordDBDM { id = Guid.NewGuid(), password = " 7;XS9c_0Ws", createdate = DateTime.Now, updatedate = DateTime.Now };

            CredentialDBDM cred1 = new CredentialDBDM { id = Guid.NewGuid(), domain = "domain3", username = "username1", remote = "web.dosadno.com", note = "Nekoja glupos930752088", createdate = DateTime.Now, updatedate = DateTime.Now, password = p1 };
            CredentialDBDM cred2 = new CredentialDBDM { id = Guid.NewGuid(), domain = "domain1", username = "username2", remote = "mail.dase.int", note = "Nekoja glupos199390833", createdate = DateTime.Now, updatedate = DateTime.Now, password = p2 };
            CredentialDBDM cred3 = new CredentialDBDM { id = Guid.NewGuid(), domain = "domain3", username = "username3", remote = "pay.novo.gov", note = "Nekoja glupos2013175463", createdate = DateTime.Now, updatedate = DateTime.Now, password = p3 };
            CredentialDBDM cred4 = new CredentialDBDM { id = Guid.NewGuid(), domain = "domain3", username = "username4", remote = "ebank.smisluvaat.int", note = "Nekoja glupos716480844", createdate = DateTime.Now, updatedate = DateTime.Now, password = p4 };
            CredentialDBDM cred5 = new CredentialDBDM { id = Guid.NewGuid(), domain = ".", username = "username5", remote = "63.46.89.37", note = "Nekoja glupos658589805", createdate = DateTime.Now, updatedate = DateTime.Now, password = p5 };
            CredentialDBDM cred6 = new CredentialDBDM { id = Guid.NewGuid(), domain = ".", username = "username6", remote = "12.38.240.64", note = "Nekoja glupos1407109890", createdate = DateTime.Now, updatedate = DateTime.Now, password = p6 };
            CredentialDBDM cred7 = new CredentialDBDM { id = Guid.NewGuid(), domain = "domain2", username = "username7", remote = "pay.smisluvaat.int", note = "Nekoja glupos2032264710", createdate = DateTime.Now, updatedate = DateTime.Now, password = p7 };
            CredentialDBDM cred8 = new CredentialDBDM { id = Guid.NewGuid(), domain = "domain3", username = "username8", remote = "site.treba.org", note = "Nekoja glupos1906180501", createdate = DateTime.Now, updatedate = DateTime.Now, password = p8 };
            CredentialDBDM cred9 = new CredentialDBDM { id = Guid.NewGuid(), domain = "domain3", username = "username9", remote = "web.ima.org", note = "Nekoja glupos453919119", createdate = DateTime.Now, updatedate = DateTime.Now, password = p9 };
            CredentialDBDM cred10 = new CredentialDBDM { id = Guid.NewGuid(), domain = ".", username = "username10", remote = "223.46.65.236", note = "Nekoja glupos1636740516", createdate = DateTime.Now, updatedate = DateTime.Now, password = p10 };
            CredentialDBDM cred11 = new CredentialDBDM { id = Guid.NewGuid(), domain = "domain3", username = "username11", remote = "shopping.dase.net", note = "Nekoja glupos1672473124", createdate = DateTime.Now, updatedate = DateTime.Now, password = p11 };
            CredentialDBDM cred12 = new CredentialDBDM { id = Guid.NewGuid(), domain = "domain1", username = "username12", remote = "shopping.novo.net", note = "Nekoja glupos788383967", createdate = DateTime.Now, updatedate = DateTime.Now, password = p12 };
            CredentialDBDM cred13 = new CredentialDBDM { id = Guid.NewGuid(), domain = "domain5", username = "username13", remote = "shopping.treba.org", note = "Nekoja glupos1752828380", createdate = DateTime.Now, updatedate = DateTime.Now, password = p13 };
            CredentialDBDM cred14 = new CredentialDBDM { id = Guid.NewGuid(), domain = ".", username = "username14", remote = "28.125.252.106", note = "Nekoja glupos647187141", createdate = DateTime.Now, updatedate = DateTime.Now, password = p14 };
            CredentialDBDM cred15 = new CredentialDBDM { id = Guid.NewGuid(), domain = ".", username = "username15", remote = "88.151.23.102", note = "Nekoja glupos578309521", createdate = DateTime.Now, updatedate = DateTime.Now, password = p15 };
            CredentialDBDM cred16 = new CredentialDBDM { id = Guid.NewGuid(), domain = "domain3", username = "username16", remote = "web.dase.com", note = "Nekoja glupos904035522", createdate = DateTime.Now, updatedate = DateTime.Now, password = p16 };
            CredentialDBDM cred17 = new CredentialDBDM { id = Guid.NewGuid(), domain = "domain3", username = "username17", remote = "web.dosadno.net", note = "Nekoja glupos216551115", createdate = DateTime.Now, updatedate = DateTime.Now, password = p17 };
            CredentialDBDM cred18 = new CredentialDBDM { id = Guid.NewGuid(), domain = "domain4", username = "username18", remote = "pay.novo.net", note = "Nekoja glupos64580569", createdate = DateTime.Now, updatedate = DateTime.Now, password = p18 };
            CredentialDBDM cred19 = new CredentialDBDM { id = Guid.NewGuid(), domain = ".", username = "username19", remote = "73.181.55.57", note = "Nekoja glupos458343822", createdate = DateTime.Now, updatedate = DateTime.Now, password = p19 };
            CredentialDBDM cred20 = new CredentialDBDM { id = Guid.NewGuid(), domain = "domain3", username = "username20", remote = "www.treba.com", note = "Nekoja glupos1177610023", createdate = DateTime.Now, updatedate = DateTime.Now, password = p20 };
            CredentialDBDM cred21 = new CredentialDBDM { id = Guid.NewGuid(), domain = "domain1", username = "username21", remote = "www.smisluvaat.int", note = "Nekoja glupos1759831662", createdate = DateTime.Now, updatedate = DateTime.Now, password = p21 };
            CredentialDBDM cred22 = new CredentialDBDM { id = Guid.NewGuid(), domain = "domain1", username = "username22", remote = "shopping.dase.gov", note = "Nekoja glupos707118051", createdate = DateTime.Now, updatedate = DateTime.Now, password = p22 };
            CredentialDBDM cred23 = new CredentialDBDM { id = Guid.NewGuid(), domain = "domain3", username = "username23", remote = "pay.ima.gov", note = "Nekoja glupos1253642279", createdate = DateTime.Now, updatedate = DateTime.Now, password = p23 };
            CredentialDBDM cred24 = new CredentialDBDM { id = Guid.NewGuid(), domain = "domain3", username = "username24", remote = "ebank.smisluvaat.mk", note = "Nekoja glupos1041563696", createdate = DateTime.Now, updatedate = DateTime.Now, password = p24 };
            CredentialDBDM cred25 = new CredentialDBDM { id = Guid.NewGuid(), domain = "domain3", username = "username25", remote = "ebank.novo.edu", note = "Nekoja glupos1459004186", createdate = DateTime.Now, updatedate = DateTime.Now, password = p25 };
            CredentialDBDM cred26 = new CredentialDBDM { id = Guid.NewGuid(), domain = ".", username = "username26", remote = "76.72.73.78", note = "Nekoja glupos484730042", createdate = DateTime.Now, updatedate = DateTime.Now, password = p26 };
            CredentialDBDM cred27 = new CredentialDBDM { id = Guid.NewGuid(), domain = "domain3", username = "username27", remote = "site.sto.int", note = "Nekoja glupos391033194", createdate = DateTime.Now, updatedate = DateTime.Now, password = p27 };
            CredentialDBDM cred28 = new CredentialDBDM { id = Guid.NewGuid(), domain = "domain5", username = "username28", remote = "web.ima.com", note = "Nekoja glupos1014217883", createdate = DateTime.Now, updatedate = DateTime.Now, password = p28 };
            CredentialDBDM cred29 = new CredentialDBDM { id = Guid.NewGuid(), domain = "domain3", username = "username29", remote = "shopping.sto.edu", note = "Nekoja glupos1594868751", createdate = DateTime.Now, updatedate = DateTime.Now, password = p29 };
            CredentialDBDM cred30 = new CredentialDBDM { id = Guid.NewGuid(), domain = "domain4", username = "username30", remote = "mail.ima.mk", note = "Nekoja glupos1699038043", createdate = DateTime.Now, updatedate = DateTime.Now, password = p30 };

            PasswordDBDM p81 = new PasswordDBDM { id = Guid.NewGuid(), password = " X_gK3k;Qb;", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p82 = new PasswordDBDM { id = Guid.NewGuid(), password = " |:;zMYU9^o", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p83 = new PasswordDBDM { id = Guid.NewGuid(), password = " !Mk-)2|sb", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p84 = new PasswordDBDM { id = Guid.NewGuid(), password = " HQNh7F0S=&", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p85 = new PasswordDBDM { id = Guid.NewGuid(), password = " FP2s2Ma}uC", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p86 = new PasswordDBDM { id = Guid.NewGuid(), password = " SVaQPPHz[", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p87 = new PasswordDBDM { id = Guid.NewGuid(), password = " j^ojbWS91E", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p88 = new PasswordDBDM { id = Guid.NewGuid(), password = " [Z[C%<?XC", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p89 = new PasswordDBDM { id = Guid.NewGuid(), password = " My.;Db1*n+", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p90 = new PasswordDBDM { id = Guid.NewGuid(), password = " Jg[hg[hL{", createdate = DateTime.Now, updatedate = DateTime.Now };
            PasswordDBDM p91 = new PasswordDBDM { id = Guid.NewGuid(), password = " b[5v9<.#N,", createdate = DateTime.Now, updatedate = DateTime.Now };


            CertificateDBDM cert81 = new CertificateDBDM { id = Guid.NewGuid(), name = "cert81", friendlyname = "web.ima.gov", issuedBy = "www.ima.int", issuedTo = "ebank.ima.com", expirationDate = DateTime.Now, password = p81 };
            CertificateDBDM cert82 = new CertificateDBDM { id = Guid.NewGuid(), name = "cert82", friendlyname = "pay.dosadno.int", issuedBy = "web.ima.net", issuedTo = "pay.dosadno.com", expirationDate = DateTime.Now, password = p82 };
            CertificateDBDM cert83 = new CertificateDBDM { id = Guid.NewGuid(), name = "cert83", friendlyname = "ebank.treba.net", issuedBy = "web.dosadno.net", issuedTo = "www.treba.mk", expirationDate = DateTime.Now, password = p83 };
            CertificateDBDM cert84 = new CertificateDBDM { id = Guid.NewGuid(), name = "cert84", friendlyname = "pay.treba.com", issuedBy = "mail.treba.mk", issuedTo = "shopping.treba.com", expirationDate = DateTime.Now, password = p84 };
            CertificateDBDM cert85 = new CertificateDBDM { id = Guid.NewGuid(), name = "cert85", friendlyname = "www.ima.gov", issuedBy = "web.novo.mk", issuedTo = "www.treba.gov", expirationDate = DateTime.Now, password = p85 };
            CertificateDBDM cert86 = new CertificateDBDM { id = Guid.NewGuid(), name = "cert86", friendlyname = "mail.sto.net", issuedBy = "mail.ima.gov", issuedTo = "pay.smisluvaat.org", expirationDate = DateTime.Now, password = p86 };
            CertificateDBDM cert87 = new CertificateDBDM { id = Guid.NewGuid(), name = "cert87", friendlyname = "ebank.smisluvaat.mk", issuedBy = "ebank.ima.net", issuedTo = "ebank.treba.com", expirationDate = DateTime.Now, password = p87 };
            CertificateDBDM cert88 = new CertificateDBDM { id = Guid.NewGuid(), name = "cert88", friendlyname = "ebank.dosadno.net", issuedBy = "pay.ima.mk", issuedTo = "web.treba.com", expirationDate = DateTime.Now, password = p88 };
            CertificateDBDM cert89 = new CertificateDBDM { id = Guid.NewGuid(), name = "cert89", friendlyname = "web.smisluvaat.edu", issuedBy = "mail.ima.edu", issuedTo = "site.dase.int", expirationDate = DateTime.Now, password = p89 };
            CertificateDBDM cert90 = new CertificateDBDM { id = Guid.NewGuid(), name = "cert90", friendlyname = "web.ima.gov", issuedBy = "site.ima.int", issuedTo = "shopping.dosadno.net", expirationDate = DateTime.Now, password = p90 };
            CertificateDBDM cert91 = new CertificateDBDM { id = Guid.NewGuid(), name = "cert91", friendlyname = "ebank.smisluvaat.com", issuedBy = "web.dosadno.net", issuedTo = "ebank.ima.com", expirationDate = DateTime.Now, password = p91 };

            t1.credentials = [cred1, cred2,cred7 ,cred8];

            t2.certificates = [cert84, cert85];
            t2.credentials = [cred3, cred4];
            t3.credentials = [cred5, cred6];
            t3.certificates = [cert86, cert87];

            t6.certificates = [cert88,cert89];
            t6.credentials = [cred15, cred16];
            u1.credentials = [cred9, cred10, cred11, cred12];

            u1.certificates = [cert81, cert82];
            u2.credentials = [cred13,cred14];
            u3.certificates = [cert83];

            _context.Clients.AddRange(c1, c2, c3, c4, c5);
            _context.Teams.AddRange(t1,t2,t3,t4,t5,t6,t7,t8,t9,t10);
            _context.Users.AddRange(u1,u2,u3,u4);
            
            _context.Passwords.AddRange(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23, p24, p25, p26, p27, p28, p29, p30, p81, p82, p83, p84, p85, p86, p87, p88, p89, p90, p91);

            _context.Certificates.AddRange(cert81, cert82, cert83, cert84, cert85, cert86, cert87, cert88, cert89, cert90, cert91);
            _context.Credentials.AddRange(cred1, cred2, cred3, cred4, cred5, cred6, cred7, cred8, cred9, cred10, cred11, cred12, cred13, cred14, cred15, cred16, cred17, cred18, cred19, cred20, cred21, cred22, cred23, cred24, cred25, cred26, cred27, cred28, cred29, cred30);

            _context.SaveChanges();
        }
    }
}
