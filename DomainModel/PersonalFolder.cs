using System;


namespace DomainModel
{
    public class PersonalFolder
    {
        public Guid id { get; set; }
        public string name { get; set; }

        public List<Credential> credentials { get; set; }
        public List<Certificate> certificates { get; set; }
    }
}
