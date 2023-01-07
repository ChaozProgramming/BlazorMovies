using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMovies.Shared.DTO
{
    public class MovieFilterDTO
    {
        public int Page { get; set; } = 1;
        public int RecordsPerPage { get; set; } = 10;
        public PaginationDTO Pagination
        {
            get { return new PaginationDTO() { Page = Page, RecordsPerPage = RecordsPerPage }; }
        }
        public string Title { get; set; } = String.Empty;
        public int GenreId { get; set; }
        public bool InTheaters { get; set; }
        public bool UpcomingReleases { get; set; }
    }
}
