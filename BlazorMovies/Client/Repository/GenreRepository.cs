using BlazorMovies.Client.Helpers;
using BlazorMovies.Shared.Entities;

namespace BlazorMovies.Client.Repository
{
    public class GenreRepository : IGenreRepository
    {
        private readonly IHttpService httpService;
        private string url = "api/genre"; //endpoint

        public GenreRepository(IHttpService httpService)
        {
            this.httpService = httpService; 
        }
        public async Task CreateGenre(Genre genre)
        {
            //var client = new swaggerClient("https://localhost:7177", httpClient);
            //var response = await client.GenresAsync(genre);

            var response = await httpService.Post(url, genre);
            if(!response.Succes)
            {
                throw new ApplicationException(await response.GetBody());
            }
            
        }

        public async Task UpdateGenre(Genre genre)
        {
            var response = await httpService.Put(url, genre);
            if (!response.Succes)
            {
                throw new ApplicationException(await response.GetBody());
            }

        }

        public async Task<List<Genre>> GetGenres()
        {
            var response = await httpService.Get<List<Genre>>(url, false);
            if (!response.Succes)
            {
                throw new ApplicationException(await response.GetBody());
            }

            return response.Response;
        }

        public async Task<Genre> GetGenre(int id)
        {
            var response = await httpService.Get<Genre>($"{url}/{id}");
            if (!response.Succes)
            {
                throw new ApplicationException(await response.GetBody());
            }

            return response.Response;
        }

        public async Task DeleteGenre(int id)
        {
            var response = await httpService.Delete($"{url}/{id}");
            if (!response.Succes)
            {
                throw new ApplicationException(await response.GetBody());
            }
        }
    }
}

