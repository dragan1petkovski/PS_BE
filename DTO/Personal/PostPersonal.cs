using System.ComponentModel.DataAnnotations;


namespace DTO.Personal
{
    public class PostPersonalFolder
    {
        [RegularExpression(@"^[a-zA-Z0-9_-]*$")]
        [Required]
        public string name {  get; set; }
    }
}
