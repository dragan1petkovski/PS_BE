using System;


namespace DomainModel.DB
{
    public class PersonalFolderDBDM
    {
        public Guid id {  get; set; }
        public string name { get; set; }

        public List<CredentialDBDM> credentials { get; set; }
        public List<CertificateDBDM> certificates { get; set; }
    }
}
