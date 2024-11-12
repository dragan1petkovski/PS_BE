namespace DTO.Client
{
    public class NavBodyClient
    {
        public Guid id { get ; set; }
        public string name { get; set; }
        public NavBodyClient() { }

        public NavBodyClient(Guid id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
