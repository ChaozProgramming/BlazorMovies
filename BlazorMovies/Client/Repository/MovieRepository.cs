using BlazorMovies.Client.Helpers;
using BlazorMovies.Shared.DTO;
using BlazorMovies.Shared.Entities;

namespace BlazorMovies.Client.Repository
{
    public class MovieRepository : IMovieRepository
    {
        private readonly IHttpService httpService;
        private string url = "api/movie"; //endpoint

        public MovieRepository(IHttpService httpService)
        {
            this.httpService = httpService; 
        }
        public async Task<int> CreateMovie(Movie movie)
        {
            var response = await httpService.Post<Movie, int>(url, movie);
            if(!response.Succes)
            {
                throw new ApplicationException(await response.GetBody());
            }

            return response.Response;


        }

        public async Task<PaginatedResponse<List<Movie>>> GetMoviesFiltered(MovieFilterDTO movieFilterDTO)
        {
            //Post<"Data type sending, "Data type receiving">
            var httpResponse = await httpService.Post<MovieFilterDTO, List<Movie>>($"{url}/filter", movieFilterDTO, false);
            if (!httpResponse.Succes)
            {
                throw new ApplicationException(await httpResponse.GetBody());
            }

            var totalAmountOfPages = int.Parse(httpResponse.HttpResponseMessage.Headers.GetValues("totalAmountPages").FirstOrDefault());
            var paginatedResponse = new PaginatedResponse<List<Movie>>()
            {
                Response = httpResponse.Response,
                TotalAmountPages = totalAmountOfPages
            };

            return paginatedResponse;
        }

        public async Task<DetailsMovieDTO> GetDetailMovieDTO(int id)
        {
            return await GetPageDTO<DetailsMovieDTO>($"{url}/{id}", false);
        }

        public async Task<MovieUpdateDTO> GetMovieForUpdate(int id)
        {
            return await GetPageDTO<MovieUpdateDTO>($"{url}/update/{id}");
        }

        public async Task<IndexPageDTO> GetIndexPageDTO()
        {
            return await GetPageDTO<IndexPageDTO>(url, false);
        }

        private async Task<T> GetPageDTO<T>(string url, bool includeToken = true)
        {
            var response = await httpService.Get<T>(url, includeToken);
            if (!response.Succes)
            {
                throw new ApplicationException(await response.GetBody());
            }

            return response.Response;


        }

        public async Task UpdateMovie(Movie movie)
        {
            var response = await httpService.Put(url, movie);
            if (!response.Succes)
            {
                throw new ApplicationException(await response.GetBody());
            }

        }

        public async Task DeleteMovie(int id)
        {
            var response = await httpService.Delete($"{url}/{id}");
            if (!response.Succes)
            {
                throw new ApplicationException(await response.GetBody());
            }
        }
    }
}

