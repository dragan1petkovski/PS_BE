namespace DTOModel.ClientDTO
{
    public class NavBodyClientDTO
    {
        public Guid id { get ; set; }
        public string name { get; set; }
        public NavBodyClientDTO() { }

        public NavBodyClientDTO(Guid id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
