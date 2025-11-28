using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public Person? Person { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Range(0,100)]
        public int Level { get; set; }
    }
}
