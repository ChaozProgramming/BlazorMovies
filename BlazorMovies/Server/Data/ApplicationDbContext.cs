using BlazorMovies.Shared.Entities;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BlazorMovies.Server.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<IdentityUser>
    {
        protected readonly IConfiguration Configuration;

        //Property name is de database table name
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<MovieActor> MoviesActors { get; set; }
        public DbSet<MovieGenre> MoviesGenres { get; set; }
        public DbSet<MovieRating> MoviesRatings { get; set; }

        public ApplicationDbContext(
            IConfiguration configuration,
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // connect to mysql with connection string from app settings
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MovieActor>().HasKey(x => new { x.MovieId, x.PersonId });
            modelBuilder.Entity<MovieGenre>().HasKey(x => new { x.MovieId, x.GenreId });

            base.OnModelCreating(modelBuilder);

            /*
            modelBuilder.Entity<Publisher>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(e => e.ISBN);
                entity.Property(e => e.Title).IsRequired();
                entity.HasOne(d => d.Publisher)
                  .WithMany(p => p.Books);
            });
            */
        }

    }
}