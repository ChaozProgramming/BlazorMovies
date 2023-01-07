
using BlazorMovies.Shared.Entities;

namespace BlazorMovies.Client.Helpers
{
    public class RepositoryInMemory : IRepository
    {
        public List<Movie> GetMovies()
        {
            return new List<Movie>()
            {
                new Movie() { Id = 1,
                            Title = "Braveheart",
                            ReleaseDate = new DateTime(2019,8,9),
                            Poster = "https://m.media-amazon.com/images/M/MV5BMzkzMmU0YTYtOWM3My00YzBmLWI0YzctOGYyNTkwMWE5MTJkXkEyXkFqcGdeQXVyNzkwMjQ5NzM@._V1_.jpg"},
                new Movie() { Id = 2, Title = "Moana", ReleaseDate = new DateTime(2020,3,23),
                Poster= "https://media.s-bol.com/ZzZ8ymP3L1Lg/550x825.jpg"},
                new Movie() { Id = 3, Title = "Inception", ReleaseDate = new DateTime(2021,4,3),
                Poster= "https://www.moviemeter.nl/images/cover/59000/59666.jpg"}

            };
        }
    }
}
