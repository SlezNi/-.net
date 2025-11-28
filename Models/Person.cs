using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class Person
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;

        public string? Bio { get; set; }
        public string? ProfileImagePath { get; set; }

        public List<Skill> Skills { get; set; } = new();
    }
}
