using AutoMapper;
using BlazorMovies.Server.Data;
using BlazorMovies.Server.Helpers;
using BlazorMovies.Shared.DTO;
using BlazorMovies.Shared.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorMovies.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")] //endpoint [controller] = name of the class "Genres"
    [Authorize(Roles = "Admin")]
    public class PersonController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IFileStorageService fileStorageService;
        private readonly IMapper mapper;

        public PersonController(ApplicationDbContext context, 
                                IFileStorageService fileStorageService,
                                IMapper mapper)
        {
            this.context = context;
            this.fileStorageService = fileStorageService;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Person person)
        {
            if (!string.IsNullOrWhiteSpace(person.Picture))
            { 
                var personPicture = Convert.FromBase64String(person.Picture);
                person.Picture = await fileStorageService.SaveFile(personPicture, ".jpg", "person");
            }

            context.Add(person);
            await context.SaveChangesAsync();
            return person.Id;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Person>>> Get([FromQuery]PaginationDTO paginationDTO)
        {
            var queryable = context.Person.AsQueryable();
            await HttpContext.InsertPaginationParametersInResponse(queryable, paginationDTO.RecordsPerPage);
            return await queryable.Paginate(paginationDTO).ToListAsync();
        }

        [HttpGet("search/{searchText}")]        
        public async Task<ActionResult<List<Person>>> FilterByName(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText)) { return new List<Person>(); }
            return await context.Person.Where(x => x.Name.Contains(searchText))
                .Take(5)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Person>> Get(int id)
        {
            var person = await context.Person.FirstOrDefaultAsync(x => x.Id == id);
            if (person == null) return NotFound();
            return person;
        }        

        [HttpPut]
        public async Task<ActionResult> Put(Person person)
        {
            var personDb = await context.Person.FirstOrDefaultAsync(x =>x.Id == person.Id);
            if(personDb == null) return NotFound();

            personDb = mapper.Map(person, personDb);

            //Person has been updated
            if (!string.IsNullOrWhiteSpace(person.Picture))
            {
                var personPicture = Convert.FromBase64String(person.Picture);
                personDb.Picture = await fileStorageService.EditFile(personPicture, ".jpg", "person", personDb.Picture);
            }
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var genre = await context.Person.FirstOrDefaultAsync(x => x.Id == id);
            if (genre == null) return NotFound();

            context.Remove(genre);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
