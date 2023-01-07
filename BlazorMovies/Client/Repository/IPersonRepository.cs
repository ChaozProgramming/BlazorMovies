
using BlazorMovies.Shared.Entities;
using BlazorMovies.Shared.DTO;

namespace BlazorMovies.Client.Repository
{
    public interface IPersonRepository
    {
        Task CreatePerson(Person person);
        Task DeletePerson(int id);
        Task<Person> GetPersonById(int id);
        Task<List<Person>> GetPersonByName(string name);
        Task<PaginatedResponse<List<Person>>> GetPersons(PaginationDTO paginationDTO);
        Task UpdatePerson(Person person);
    }
}
