using DomainModel;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using EncryptionLayer;
using Microsoft.Extensions.Configuration;
using System.Text;
using TransitionObjectMapper;

namespace DataAccessLayerDB
{
    public class PSDBInitializer
    {
        private readonly PSDBContext _context;
        private readonly IConfiguration _configuration;

        public PSDBInitializer(PSDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public void Run()
        {
            Client c1 = new Client { id = Guid.NewGuid(), name = "Client1", createdate = DateTime.Now, updatedate = DateTime.Now };
            Client c2 = new Client { id = Guid.NewGuid(), name = "Client2", createdate = DateTime.Now, updatedate = DateTime.Now };
            Client c3 = new Client { id = Guid.NewGuid(), name = "Client3", createdate = DateTime.Now, updatedate = DateTime.Now };
            Client c4 = new Client { id = Guid.NewGuid(), name = "Client4", createdate = DateTime.Now, updatedate = DateTime.Now };
            Client c5 = new Client { id = Guid.NewGuid(), name = "Client5", createdate = DateTime.Now, updatedate = DateTime.Now };

            Team t1 = new Team { id = Guid.NewGuid(), name = "Team1", createdate = DateTime.Now, updatedate = DateTime.Now };
            Team t2 = new Team { id = Guid.NewGuid(), name = "Team2", createdate = DateTime.Now, updatedate = DateTime.Now };
            Team t3 = new Team { id = Guid.NewGuid(), name = "Team3", createdate = DateTime.Now, updatedate = DateTime.Now };
            Team t4 = new Team { id = Guid.NewGuid(), name = "Team4", createdate = DateTime.Now, updatedate = DateTime.Now };
            Team t5 = new Team { id = Guid.NewGuid(), name = "Team5", createdate = DateTime.Now, updatedate = DateTime.Now };
            Team t6 = new Team { id = Guid.NewGuid(), name = "Team6", createdate = DateTime.Now, updatedate = DateTime.Now };
            Team t7 = new Team { id = Guid.NewGuid(), name = "Team7", createdate = DateTime.Now, updatedate = DateTime.Now };
            Team t8 = new Team { id = Guid.NewGuid(), name = "Team8", createdate = DateTime.Now, updatedate = DateTime.Now };
            Team t9 = new Team { id = Guid.NewGuid(), name = "Team9", createdate = DateTime.Now, updatedate = DateTime.Now };
            Team t10 = new Team { id = Guid.NewGuid(), name = "Team10", createdate = DateTime.Now, updatedate = DateTime.Now };
            Team t11 = new Team { id = Guid.NewGuid(), name = "Team1", createdate = DateTime.Now, updatedate = DateTime.Now };
            Team t12 = new Team { id = Guid.NewGuid(), name = "Team2", createdate = DateTime.Now, updatedate = DateTime.Now };
            Team t13 = new Team { id = Guid.NewGuid(), name = "Team3", createdate = DateTime.Now, updatedate = DateTime.Now };
            Team t14 = new Team { id = Guid.NewGuid(), name = "Team4", createdate = DateTime.Now, updatedate = DateTime.Now };
            Team t15 = new Team { id = Guid.NewGuid(), name = "Team5", createdate = DateTime.Now, updatedate = DateTime.Now };

            PasswordHasher<User> passwordHasher = new PasswordHasher<User>();
			SymmetricEncryption se = new SymmetricEncryption();

			IdentityRole adminRole = new IdentityRole();
            adminRole.Name = "Administrator";
            adminRole.NormalizedName = adminRole.Name.Normalize();
            adminRole.Id = Guid.NewGuid().ToString();
            
            IdentityRole userRole = new IdentityRole();
            userRole.Name = "User";
            userRole.NormalizedName = userRole.Name.Normalize();
            userRole.Id = Guid.NewGuid().ToString();
            
            User u1 = new User();
            u1.firstname = "User1";
            u1.lastname = "userski1";
            u1.UserName = "User1";
            u1.NormalizedUserName = u1.UserName.Normalize();
            u1.Id = Guid.NewGuid().ToString();
            u1.Email = "user1.userski1@test.com";
            u1.NormalizedEmail = u1.Email.Normalize();
            u1.PasswordHash = passwordHasher.HashPassword(u1, "user1");
            u1.createdate = DateTime.Now;
            u1.updatedate = DateTime.Now;

			User u2 = new User();
			u2.firstname = "User2";
			u2.lastname = "userski2";
			u2.UserName = "User2";
			u2.NormalizedUserName = u2.UserName.Normalize();
			u2.Id = Guid.NewGuid().ToString();
			u2.Email = "user2.userski2@test.com";
			u2.NormalizedEmail = u2.Email.Normalize();
			u2.PasswordHash = passwordHasher.HashPassword(u2, "user2");
			u2.createdate = DateTime.Now;
			u2.updatedate = DateTime.Now;

			User u3 = new User();
			u3.firstname = "user3";
			u3.lastname = "userski3";
			u3.UserName = "User3";
			u3.NormalizedUserName = u3.UserName.Normalize();
			u3.Id = Guid.NewGuid().ToString();
			u3.Email = "user3.userski3@test.com";
			u3.NormalizedEmail = u3.Email.Normalize();
			u3.PasswordHash = passwordHasher.HashPassword(u3, "user3");
			u3.createdate = DateTime.Now;
			u3.updatedate = DateTime.Now;

			User u4 = new User();
			u4.firstname = "user4";
			u4.lastname = "userski4";
			u4.UserName = "User4";
			u4.NormalizedUserName = u4.UserName.Normalize();
			u4.Id = Guid.NewGuid().ToString();
			u4.Email = "user4.userski4@test.com";
			u4.NormalizedEmail = u4.Email.Normalize();
			u4.PasswordHash = passwordHasher.HashPassword(u4, "user4");
			u4.createdate = DateTime.Now;
			u4.updatedate = DateTime.Now;

			User u5 = new User();
			u5.firstname = "user5";
			u5.lastname = "userski5";
			u5.UserName = "User5";
			u5.NormalizedUserName = u5.UserName.Normalize();
			u5.Id = Guid.NewGuid().ToString();
			u5.Email = "user5.userski5@test.com";
			u5.NormalizedEmail = u5.Email.Normalize();
			u5.PasswordHash = passwordHasher.HashPassword(u5, "user5");
			u5.createdate = DateTime.Now;
			u5.updatedate = DateTime.Now;


			IdentityUserRole<string> userRole5 = new IdentityUserRole<string>();
			userRole5.RoleId = userRole.Id;
			userRole5.UserId = u5.Id;

            IdentityUserRole<string> userRole2 = new IdentityUserRole<string>();
			userRole2.RoleId = userRole.Id;
			userRole2.UserId = u2.Id;

			IdentityUserRole<string> adminUser = new IdentityUserRole<string>();
			adminUser.RoleId = adminRole.Id;
			adminUser.UserId = u3.Id;

			IdentityUserRole<string> userRole4 = new IdentityUserRole<string>();
			userRole4.RoleId = userRole.Id;
			userRole4.UserId = u4.Id;

			IdentityUserRole<string> userRole1 = new IdentityUserRole<string>();
			userRole1.RoleId = userRole.Id;
			userRole1.UserId = u1.Id;

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

            SymmetricKey key = se.EncryptString("|7{25^{,Qh", _configuration);
            Password p1 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

            key = se.EncryptString("&+}ELFw7OX", _configuration);
			Password p2 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("*6_),pS.92", _configuration);
			Password p3 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("<%D;]Da]r", _configuration);
            Password p4 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("laOp3sl59X", _configuration);
			Password p5 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("+Ua|bHF>{", _configuration);
			Password p6 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("9Rj.&rN*", _configuration);
			Password p7 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("+8<z6hhK@<", _configuration);
			Password p8 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("S)40c5Vf66", _configuration);
			Password p9 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("gwr)_<Z?d", _configuration);
			Password p10 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("gsMkJKqxlo", _configuration);
			Password p11 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("=_O]!4_zAI", _configuration);
			Password p12 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("Ku2NM-K602", _configuration);
			Password p13 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("fP(iWME.sV", _configuration);
			Password p14 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("PU((c,,|vs", _configuration);
			Password p15 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("4;Rrb3O8<Q", _configuration);
			Password p16 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("Mxcuue4lM)", _configuration);
			Password p17 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("M6]jlAM=Ab", _configuration);
            Password p18 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("sg*m4z:F99", _configuration);
			Password p19 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("|,Y3V**D}w", _configuration);
			Password p20 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("e^rQu_Ed+2", _configuration);
			Password p21 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("443,+E>}kR", _configuration);
			Password p22 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("zF]Dhpog2M", _configuration);
			Password p23 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString(".3^LjZ5f|[", _configuration);
			Password p24 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("r-;Ph[OEP#", _configuration);
			Password p25 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("yz<@p-l]H", _configuration);
			Password p26 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("Qy||tx1OH-", _configuration);
			Password p27 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("+Ov%;:IfHp", _configuration);
			Password p28 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("6PkJ5#0iAc", _configuration);
			Password p29 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("7;XS9c_0Ws", _configuration);
			Password p30 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

            Credential cred1 = new Credential { id = Guid.NewGuid(), domain = "domain3", username = "username1", remote = "web.dosadno.com", note = "Nekoja glupos930752088", createdate = DateTime.Now, updatedate = DateTime.Now, password = p1 };
            Credential cred2 = new Credential { id = Guid.NewGuid(), domain = "domain1", username = "username2", remote = "mail.dase.int", note = "Nekoja glupos199390833", createdate = DateTime.Now, updatedate = DateTime.Now, password = p2 };
            Credential cred3 = new Credential { id = Guid.NewGuid(), domain = "domain3", username = "username3", remote = "pay.novo.gov", note = "Nekoja glupos2013175463", createdate = DateTime.Now, updatedate = DateTime.Now, password = p3 };
            Credential cred4 = new Credential { id = Guid.NewGuid(), domain = "domain3", username = "username4", remote = "ebank.smisluvaat.int", note = "Nekoja glupos716480844", createdate = DateTime.Now, updatedate = DateTime.Now, password = p4 };
            Credential cred5 = new Credential { id = Guid.NewGuid(), domain = ".", username = "username5", remote = "63.46.89.37", note = "Nekoja glupos658589805", createdate = DateTime.Now, updatedate = DateTime.Now, password = p5 };
            Credential cred6 = new Credential { id = Guid.NewGuid(), domain = ".", username = "username6", remote = "12.38.240.64", note = "Nekoja glupos1407109890", createdate = DateTime.Now, updatedate = DateTime.Now, password = p6 };
            Credential cred7 = new Credential { id = Guid.NewGuid(), domain = "domain2", username = "username7", remote = "pay.smisluvaat.int", note = "Nekoja glupos2032264710", createdate = DateTime.Now, updatedate = DateTime.Now, password = p7 };
            Credential cred8 = new Credential { id = Guid.NewGuid(), domain = "domain3", username = "username8", remote = "site.treba.org", note = "Nekoja glupos1906180501", createdate = DateTime.Now, updatedate = DateTime.Now, password = p8 };
            Credential cred9 = new Credential { id = Guid.NewGuid(), domain = "domain3", username = "username9", remote = "web.ima.org", note = "Nekoja glupos453919119", createdate = DateTime.Now, updatedate = DateTime.Now, password = p9 };
            Credential cred10 = new Credential { id = Guid.NewGuid(), domain = ".", username = "username10", remote = "223.46.65.236", note = "Nekoja glupos1636740516", createdate = DateTime.Now, updatedate = DateTime.Now, password = p10 };
            Credential cred11 = new Credential { id = Guid.NewGuid(), domain = "domain3", username = "username11", remote = "shopping.dase.net", note = "Nekoja glupos1672473124", createdate = DateTime.Now, updatedate = DateTime.Now, password = p11 };
            Credential cred12 = new Credential { id = Guid.NewGuid(), domain = "domain1", username = "username12", remote = "shopping.novo.net", note = "Nekoja glupos788383967", createdate = DateTime.Now, updatedate = DateTime.Now, password = p12 };
            Credential cred13 = new Credential { id = Guid.NewGuid(), domain = "domain5", username = "username13", remote = "shopping.treba.org", note = "Nekoja glupos1752828380", createdate = DateTime.Now, updatedate = DateTime.Now, password = p13 };
            Credential cred14 = new Credential { id = Guid.NewGuid(), domain = ".", username = "username14", remote = "28.125.252.106", note = "Nekoja glupos647187141", createdate = DateTime.Now, updatedate = DateTime.Now, password = p14 };
            Credential cred15 = new Credential { id = Guid.NewGuid(), domain = ".", username = "username15", remote = "88.151.23.102", note = "Nekoja glupos578309521", createdate = DateTime.Now, updatedate = DateTime.Now, password = p15 };
            Credential cred16 = new Credential { id = Guid.NewGuid(), domain = "domain3", username = "username16", remote = "web.dase.com", note = "Nekoja glupos904035522", createdate = DateTime.Now, updatedate = DateTime.Now, password = p16 };
            Credential cred17 = new Credential { id = Guid.NewGuid(), domain = "domain3", username = "username17", remote = "web.dosadno.net", note = "Nekoja glupos216551115", createdate = DateTime.Now, updatedate = DateTime.Now, password = p17 };
            Credential cred18 = new Credential { id = Guid.NewGuid(), domain = "domain4", username = "username18", remote = "pay.novo.net", note = "Nekoja glupos64580569", createdate = DateTime.Now, updatedate = DateTime.Now, password = p18 };
            Credential cred19 = new Credential { id = Guid.NewGuid(), domain = ".", username = "username19", remote = "73.181.55.57", note = "Nekoja glupos458343822", createdate = DateTime.Now, updatedate = DateTime.Now, password = p19 };
            Credential cred20 = new Credential { id = Guid.NewGuid(), domain = "domain3", username = "username20", remote = "www.treba.com", note = "Nekoja glupos1177610023", createdate = DateTime.Now, updatedate = DateTime.Now, password = p20 };
            Credential cred21 = new Credential { id = Guid.NewGuid(), domain = "domain1", username = "username21", remote = "www.smisluvaat.int", note = "Nekoja glupos1759831662", createdate = DateTime.Now, updatedate = DateTime.Now, password = p21 };
            Credential cred22 = new Credential { id = Guid.NewGuid(), domain = "domain1", username = "username22", remote = "shopping.dase.gov", note = "Nekoja glupos707118051", createdate = DateTime.Now, updatedate = DateTime.Now, password = p22 };
            Credential cred23 = new Credential { id = Guid.NewGuid(), domain = "domain3", username = "username23", remote = "pay.ima.gov", note = "Nekoja glupos1253642279", createdate = DateTime.Now, updatedate = DateTime.Now, password = p23 };
            Credential cred24 = new Credential { id = Guid.NewGuid(), domain = "domain3", username = "username24", remote = "ebank.smisluvaat.mk", note = "Nekoja glupos1041563696", createdate = DateTime.Now, updatedate = DateTime.Now, password = p24 };
            Credential cred25 = new Credential { id = Guid.NewGuid(), domain = "domain3", username = "username25", remote = "ebank.novo.edu", note = "Nekoja glupos1459004186", createdate = DateTime.Now, updatedate = DateTime.Now, password = p25 };
            Credential cred26 = new Credential { id = Guid.NewGuid(), domain = ".", username = "username26", remote = "76.72.73.78", note = "Nekoja glupos484730042", createdate = DateTime.Now, updatedate = DateTime.Now, password = p26 };
            Credential cred27 = new Credential { id = Guid.NewGuid(), domain = "domain3", username = "username27", remote = "site.sto.int", note = "Nekoja glupos391033194", createdate = DateTime.Now, updatedate = DateTime.Now, password = p27 };
            Credential cred28 = new Credential { id = Guid.NewGuid(), domain = "domain5", username = "username28", remote = "web.ima.com", note = "Nekoja glupos1014217883", createdate = DateTime.Now, updatedate = DateTime.Now, password = p28 };
            Credential cred29 = new Credential { id = Guid.NewGuid(), domain = "domain3", username = "username29", remote = "shopping.sto.edu", note = "Nekoja glupos1594868751", createdate = DateTime.Now, updatedate = DateTime.Now, password = p29 };
            Credential cred30 = new Credential { id = Guid.NewGuid(), domain = "domain4", username = "username30", remote = "mail.ima.mk", note = "Nekoja glupos1699038043", createdate = DateTime.Now, updatedate = DateTime.Now, password = p30 };

			key = se.EncryptString("X_gK3k;Qb;", _configuration);
			Password p81 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("|:;zMYU9^o", _configuration);
			Password p82 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("!Mk-)2|sb", _configuration);
			Password p83 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("HQNh7F0S=&", _configuration);
			Password p84 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("FP2s2Ma}uC", _configuration);
			Password p85 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("SVaQPPHz[", _configuration);
			Password p86 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("j^ojbWS91E", _configuration);
			Password p87 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("[Z[C%<?XC", _configuration);
			Password p88 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("My.;Db1*n+", _configuration);
			Password p89 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("Jg[hg[hL{", _configuration);
			Password p90 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };

			key = se.EncryptString("b[5v9<.#N,", _configuration);
			Password p91 = new Password { id = Guid.NewGuid(), password = key.password, aad = key.aad, createdate = DateTime.Now, updatedate = DateTime.Now };


            Certificate cert81 = new Certificate { id = Guid.NewGuid(), name = "cert81", friendlyname = "web.ima.gov", issuedBy = "www.ima.int", issuedTo = "ebank.ima.com", expirationDate = DateTime.Now, password = p81 };
            Certificate cert82 = new Certificate { id = Guid.NewGuid(), name = "cert82", friendlyname = "pay.dosadno.int", issuedBy = "web.ima.net", issuedTo = "pay.dosadno.com", expirationDate = DateTime.Now, password = p82 };
            Certificate cert83 = new Certificate { id = Guid.NewGuid(), name = "cert83", friendlyname = "ebank.treba.net", issuedBy = "web.dosadno.net", issuedTo = "www.treba.mk", expirationDate = DateTime.Now, password = p83 };
            Certificate cert84 = new Certificate { id = Guid.NewGuid(), name = "cert84", friendlyname = "pay.treba.com", issuedBy = "mail.treba.mk", issuedTo = "shopping.treba.com", expirationDate = DateTime.Now, password = p84 };
            Certificate cert85 = new Certificate { id = Guid.NewGuid(), name = "cert85", friendlyname = "www.ima.gov", issuedBy = "web.novo.mk", issuedTo = "www.treba.gov", expirationDate = DateTime.Now, password = p85 };
            Certificate cert86 = new Certificate { id = Guid.NewGuid(), name = "cert86", friendlyname = "mail.sto.net", issuedBy = "mail.ima.gov", issuedTo = "pay.smisluvaat.org", expirationDate = DateTime.Now, password = p86 };
            Certificate cert87 = new Certificate { id = Guid.NewGuid(), name = "cert87", friendlyname = "ebank.smisluvaat.mk", issuedBy = "ebank.ima.net", issuedTo = "ebank.treba.com", expirationDate = DateTime.Now, password = p87 };
            Certificate cert88 = new Certificate { id = Guid.NewGuid(), name = "cert88", friendlyname = "ebank.dosadno.net", issuedBy = "pay.ima.mk", issuedTo = "web.treba.com", expirationDate = DateTime.Now, password = p88 };
            Certificate cert89 = new Certificate { id = Guid.NewGuid(), name = "cert89", friendlyname = "web.smisluvaat.edu", issuedBy = "mail.ima.edu", issuedTo = "site.dase.int", expirationDate = DateTime.Now, password = p89 };
            Certificate cert90 = new Certificate { id = Guid.NewGuid(), name = "cert90", friendlyname = "web.ima.gov", issuedBy = "site.ima.int", issuedTo = "shopping.dosadno.net", expirationDate = DateTime.Now, password = p90 };
            Certificate cert91 = new Certificate { id = Guid.NewGuid(), name = "cert91", friendlyname = "ebank.smisluvaat.com", issuedBy = "web.dosadno.net", issuedTo = "ebank.ima.com", expirationDate = DateTime.Now, password = p91 };

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
			_context.Teams.AddRange(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);

			_context.Users.AddRange(u1, u2, u3, u4, u5);
            _context.Roles.AddRange(userRole, adminRole);
			_context.UserRoles.AddRange(userRole2, userRole5, userRole4, userRole1, adminUser);


            
            
            _context.Passwords.AddRange(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23, p24, p25, p26, p27, p28, p29, p30, p81, p82, p83, p84, p85, p86, p87, p88, p89, p90, p91);

            _context.Certificates.AddRange(cert81, cert82, cert83, cert84, cert85, cert86, cert87, cert88, cert89, cert90, cert91);
            _context.Credentials.AddRange(cred1, cred2, cred3, cred4, cred5, cred6, cred7, cred8, cred9, cred10, cred11, cred12, cred13, cred14, cred15, cred16, cred17, cred18, cred19, cred20, cred21, cred22, cred23, cred24, cred25, cred26, cred27, cred28, cred29, cred30);

            _context.SaveChanges();
        }
    }
}
