using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorMovies.Shared.Entities
{
    public class Person
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "This is field is required")]
        public string Name { get; set; }
        public string Biography { get; set; }
        public string Picture { get; set; }
        [Required]
        public DateTime? DateOfBirth { get; set; } //= DateTime.Today.AddYears(-20);

        [NotMapped] //used to not map the prop into to database (dbContext)
        public string Character { get; set; }
        public List<MovieActor> MovieActor { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Person p2)
            {
                return Id == p2.Id;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
