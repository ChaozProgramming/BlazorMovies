using Microsoft.EntityFrameworkCore;

namespace BlazorMovies.Server.Helpers
{
    public static class HttpContextExtensions
    {
        public async static Task InsertPaginationParametersInResponse<T>(this HttpContext httpContext,
            IQueryable<T> queryable, int recordsPerPage)
        { 
            if(httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            //Count records from the querable 
            double count = await queryable.CountAsync();

            if(recordsPerPage == 0)
                recordsPerPage = 10; 

            double totalAmountPages = Math.Ceiling(count / recordsPerPage);


            //Write the result in header of the http respons
            httpContext.Response.Headers.Add("totalAmountPages", totalAmountPages.ToString());
        }
    }
}
