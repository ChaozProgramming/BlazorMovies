using BlazorMovies.Client.Helpers;
using BlazorMovies.Shared.DTO;
using BlazorMovies.Shared.Entities;

namespace BlazorMovies.Client.Repository
{
    public class PersonRepository : IPersonRepository
    {
        private readonly IHttpService httpService;
        private string url = "api/person"; //endpoint

        public PersonRepository(IHttpService httpService)
        {
            this.httpService = httpService;
        }
        public async Task CreatePerson(Person person)
        {
            var response = await httpService.Post(url, person);
            if (!response.Succes)
            {
                throw new ApplicationException(await response.GetBody());
            }

        }

        public async Task UpdatePerson(Person person)
        {
            var response = await httpService.Put(url, person);
            if (!response.Succes)
            {
                throw new ApplicationException(await response.GetBody());
            }

        }

        public async Task<PaginatedResponse<List<Person>>> GetPersons(PaginationDTO paginationDTO)
        {
            return await httpService.GetExtension<List<Person>>(url, paginationDTO);
        }

        public async Task<List<Person>> GetPersonByName(string name)
        {
            var response = await httpService.Get<List<Person>>($"{url}/search/{name}");
            if (!response.Succes)
            {
                throw new ApplicationException(await response.GetBody());
            }

            return response.Response;
        }

        public async Task<Person> GetPersonById(int id)
        {
            var response = await httpService.Get<Person>($"{url}/{id}");
            if (!response.Succes)
            {
                throw new ApplicationException(await response.GetBody());
            }

            return response.Response;
        }

        public async Task DeletePerson(int id)
        {
            var response = await httpService.Delete($"{url}/{id}");
            if (!response.Succes)
            {
                throw new ApplicationException(await response.GetBody());
            }
        }
    }
}

