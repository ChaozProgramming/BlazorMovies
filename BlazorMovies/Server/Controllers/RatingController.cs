using BlazorMovies.Server.Data;
using BlazorMovies.Shared.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorMovies.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]
    public class RatingController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;

        public RatingController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager) //retrieve user information
        {
            this.context = context;
            this.userManager = userManager;
        }

        [HttpPost]
        public async Task<ActionResult> Rate(MovieRating movieRating)
        {
            var user = await userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
            var userId = user.Id;

            var currentRating = await context.MoviesRatings
                .FirstOrDefaultAsync(x => x.MovieId == movieRating.MovieId &&
                x.UserId == userId);

            if (currentRating == null)
            {
                movieRating.UserId = userId;
                movieRating.RatingDate = DateTime.Today;
                context.Add(movieRating);
                await context.SaveChangesAsync();
            }
            else
            {
                currentRating.Rate = movieRating.Rate;
                await context.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}
