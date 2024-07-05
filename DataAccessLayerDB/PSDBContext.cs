using Microsoft.EntityFrameworkCore;
using DataAccessLayerDB;
using DomainModel.DB;
namespace DataAccessLayerDB
{
    public class PSDBContext: DbContext
    {
        public DbSet<UserDBDM> Users { get; set; }
        public DbSet<TeamDBDM> Teams { get; set; }
        public DbSet<PasswordDBDM> Passwords { get; set; }
        public DbSet<CredentialDBDM> Credentials { get; set; }
        public DbSet<ClientDBDM> Clients { get; set; }
        public DbSet<CertificateDBDM> Certificates { get; set; }

        public DbSet<PersonalFolderDBDM> PersonalFolders { get; set; }

        public PSDBContext() {}
        public PSDBContext(DbContextOptions<PSDBContext> options) : base(options) { }
    }
}
