using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMovies.Shared.Entities
{
    public class MovieRating
    {
        public int Id { get; set; }
        public int Rate { get; set; }
        public DateTime RatingDate { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public string UserId { get; set; }
    }
}
