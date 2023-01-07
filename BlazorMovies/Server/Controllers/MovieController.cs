using AutoMapper;
using BlazorMovies.Server.Data;
using BlazorMovies.Server.Helpers;
using BlazorMovies.Shared.DTO;
using BlazorMovies.Shared.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorMovies.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")] //endpoint [controller] = name of the class "Genres"    
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IFileStorageService fileStorageService;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public MovieController(ApplicationDbContext context,
                                IFileStorageService fileStorageService,
                                IMapper mapper,
                                UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.fileStorageService = fileStorageService;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<int>> Post(Movie movie)
        {
            if (!string.IsNullOrWhiteSpace(movie.Poster))
            {
                var moviePoster = Convert.FromBase64String(movie.Poster);
                movie.Poster = await fileStorageService.SaveFile(moviePoster, ".jpg", "movie");
            }

            if (movie.MovieActor != null)
            {
                for (int i = 0; i < movie.MovieActor.Count; i++)
                {
                    movie.MovieActor[i].Order = i + 1;
                }
            }

            context.Add(movie);
            await context.SaveChangesAsync();
            return movie.Id;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IndexPageDTO>> Get()
        {
            var limit = 6;

            var moviesInTheaters = await context.Movies
                .Where(x => x.InTheaters)
                .Take(limit)
                .OrderByDescending(x => x.ReleaseDate)
                .ToListAsync();

            var todaysDate = DateTime.Today;
            var upcomingReleases = await context.Movies
                .Where(x => x.ReleaseDate > todaysDate)
                .Take(limit)
                .OrderByDescending(x => x.ReleaseDate)
                .ToListAsync();

            var response = new IndexPageDTO();
            response.InTheaters = moviesInTheaters;
            response.UpcomingReleases = upcomingReleases;

            return response;


        }

        [HttpPost("filter")]
        [AllowAnonymous]

        public async Task<ActionResult<List<Movie>>> Filter(MovieFilterDTO filterMovieDTO)
        {
            var moviesQueryable = context.Movies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterMovieDTO.Title))
            {
                moviesQueryable = moviesQueryable
                    .Where(x => x.Title.Contains(filterMovieDTO.Title));
            }

            if (filterMovieDTO.InTheaters)
            {
                moviesQueryable = moviesQueryable.Where(x => x.InTheaters);
            }

            if (filterMovieDTO.UpcomingReleases)
            {
                var today = DateTime.Today;
                moviesQueryable = moviesQueryable.Where(x => x.ReleaseDate > today);
            }

            if (filterMovieDTO.GenreId != 0)
            {
                moviesQueryable = moviesQueryable
                    .Where(x => x.MovieGenre.Select(y => y.GenreId)
                    .Contains(filterMovieDTO.GenreId));
            }

            await HttpContext.InsertPaginationParametersInResponse(moviesQueryable,
                filterMovieDTO.RecordsPerPage);

            var movies = await moviesQueryable.Paginate(filterMovieDTO.Pagination).ToListAsync();

            return movies;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]

        public async Task<ActionResult<DetailsMovieDTO>> Get(int id)
        {
            var movie = await context.Movies
                .Where(x => x.Id == id)
                .Include(x => x.MovieGenre).ThenInclude(x => x.Genre)
                .Include(x => x.MovieActor).ThenInclude(x => x.Person)
                .FirstOrDefaultAsync();

            if (movie == null)
                return NotFound();

            var voteAverage = 0.0;
            var uservote = 0;

            if (await context.MoviesRatings.AnyAsync(x => x.MovieId == id))
            {
                voteAverage = await context.MoviesRatings.Where(x => x.MovieId == id)
                    .AverageAsync(x => x.Rate);

                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    var user = await userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
                    var userId = user.Id;

                    var userVoteDB = await context.MoviesRatings
                        .FirstOrDefaultAsync(x => x.MovieId == id && x.UserId == userId);

                    if (userVoteDB != null)
                    {
                        uservote = userVoteDB.Rate;
                    }
                }
            }

            movie.MovieActor = movie.MovieActor.OrderBy(x => x.Order).ToList();

            var response = new DetailsMovieDTO();
            response.movie = movie;
            response.Genres = movie.MovieGenre.Select(x => x.Genre).ToList();
            response.Actors = movie.MovieActor.Select(x =>
                new Person
                {
                    Name = x.Person.Name,
                    Picture = x.Person.Picture,
                    Character = x.Character,
                    Id = x.Person.Id
                }).ToList();
            response.UserVote = uservote;
            response.AverageVote = voteAverage;

            return response;


        }

        [HttpGet("update/{id}")]

        public async Task<ActionResult<MovieUpdateDTO>> PutGet(int id)
        {
            var movieActionResult = await Get(id);
            if (movieActionResult.Result is NotFoundResult) return NotFound();

            var movieDetailDTO = movieActionResult.Value;
            var selectedGenres = movieDetailDTO.Genres.Select(x => x.Id).ToList();
            var notSelectedGenres = await context.Genres.Where(x => !selectedGenres.Contains(x.Id)).ToListAsync();

            var model = new MovieUpdateDTO();
            model.Movie = movieDetailDTO.movie;
            model.SelectedGenres = movieDetailDTO.Genres;
            model.NotSelectedGenres = notSelectedGenres;
            model.Actors = movieDetailDTO.Actors;
            return model;

        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Put(Movie movie)
        {
            var movieDb = await context.Movies.FirstOrDefaultAsync(x => x.Id == movie.Id);
            if (movieDb == null) return NotFound();

            movieDb = mapper.Map(movie, movieDb);

            if (!string.IsNullOrWhiteSpace(movie.Poster))
            {
                var personPicture = Convert.FromBase64String(movie.Poster);
                movieDb.Poster = await fileStorageService.EditFile(personPicture, ".jpg", "movie", movieDb.Poster);
            }

            await context.Database.ExecuteSqlRawAsync($"delete from MoviesActors where MovieId={movie.Id}; delete from MoviesGenres where MovieId = {movie.Id}");

            if (movie.MovieActor != null)
            {
                for (int i = 0; i < movie.MovieActor.Count; i++)
                {
                    movie.MovieActor[i].Order = i + 1;
                }
            }

            movieDb.MovieActor = movie.MovieActor;
            movieDb.MovieGenre = movie.MovieGenre;

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var genre = await context.Movies.FirstOrDefaultAsync(x => x.Id == id);
            if (genre == null) return NotFound();

            context.Remove(genre);
            await context.SaveChangesAsync();
            return NoContent();
        }


    }
}
