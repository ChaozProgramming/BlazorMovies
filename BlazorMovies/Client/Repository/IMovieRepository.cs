
using BlazorMovies.Shared.DTO;
using BlazorMovies.Shared.Entities;

namespace BlazorMovies.Client.Repository
{
    public interface IMovieRepository
    {
        Task<int> CreateMovie(Movie movie);
        Task DeleteMovie(int id);
        Task<DetailsMovieDTO> GetDetailMovieDTO(int id);
        Task<IndexPageDTO> GetIndexPageDTO();
        Task<MovieUpdateDTO> GetMovieForUpdate(int id);
        Task<PaginatedResponse<List<Movie>>> GetMoviesFiltered(MovieFilterDTO movieFilterDTO);
        Task UpdateMovie(Movie movie);
    }
}
