using BlazorMovies.Shared.DTO;

namespace BlazorMovies.Server.Helpers
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginationDTO paginationDTO)
        {
            return queryable
                .Skip((paginationDTO.Page - 1) * paginationDTO.RecordsPerPage) //Skip a certain amount of records
                .Take(paginationDTO.RecordsPerPage); //take a certain amount of records
        }
    }
}
