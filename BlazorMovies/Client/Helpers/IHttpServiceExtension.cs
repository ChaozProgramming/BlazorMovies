using BlazorMovies.Shared.DTO;

namespace BlazorMovies.Client.Helpers
{
    public static class IHttpServiceExtension
    {
        public static async Task<T> GetExtension<T>(this IHttpService httpService, string url, bool includeToken = true)
        {
            var response = await httpService.Get<T>(url, includeToken);
            if (!response.Succes)
                throw new ApplicationException(await response.GetBody());
            return response.Response;

        }

        public static async Task<PaginatedResponse<T>> GetExtension<T>(this IHttpService httpService, string url, PaginationDTO paginationDTO, bool includeToken = true)
        {
            string newURL = "";
            if(url.Contains("?"))
                newURL = $"{url}&page={paginationDTO.Page}&recordsPerPage={paginationDTO.RecordsPerPage}";
            else
                newURL = $"{url}?page={paginationDTO.Page}&recordsPerPage={paginationDTO.RecordsPerPage}";

            var httpResponse = await httpService.Get<T>(newURL, includeToken);
            var totalAmountOfPages = int.Parse(httpResponse.HttpResponseMessage.Headers.GetValues("totalAmountPages").FirstOrDefault());
            var paginatedResponse = new PaginatedResponse<T>
            {
                Response = httpResponse.Response,
                TotalAmountPages = totalAmountOfPages
            };

            return paginatedResponse;

        }
    }
}
