using BlazorMovies.Server.Data;
using BlazorMovies.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorMovies.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")] //endpoint [controller] = name of the class "Genres"
    [Authorize(Roles = "Admin")]
    public class GenreController : Controller
    {
        private readonly ApplicationDbContext context;
        public GenreController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Genre genre)
        {
            context.Add(genre);
            await context.SaveChangesAsync();
            return genre.Id;
        }

        [HttpPut]
        public async Task<ActionResult> Put(Genre genre)
        {
            context.Attach(genre).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Genre>>> Get()
        {
            return await context.Genres.ToListAsync();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Genre>> Get(int id)
        {
            var genre = await context.Genres.FirstOrDefaultAsync(x => x.Id == id);  
            if(genre == null) return NotFound();
            return genre;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var genre = await context.Genres.FirstOrDefaultAsync(x => x.Id == id);
            if (genre == null) return NotFound();
            
            context.Remove(genre);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
