using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DataAccessLayerDB;
using DomainModel;
using Microsoft.EntityFrameworkCore;
namespace DataAccessLayerDB
{
    public class PSDBContext: IdentityDbContext<User>
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Password> Passwords { get; set; }
        public DbSet<Credential> Credentials { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<CertificateFile> CertificatesFile { get; set; }
        public DbSet<PersonalFolder> PersonalFolders { get; set; }
        public DbSet<EmailNotification> EmailNotifiers { get; set; }
        public DbSet<DeleteVerification> deleteVerifications { get; set; }
        public PSDBContext() {}
        public PSDBContext(DbContextOptions<PSDBContext> options) : base(options) { }
    }
}
