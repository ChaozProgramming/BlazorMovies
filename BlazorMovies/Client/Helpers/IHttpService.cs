namespace BlazorMovies.Client.Helpers
{
    public interface IHttpService
    {
        Task<HttpResponseWrapper<object>> Post<T>(string url, T data);
        Task<HttpResponseWrapper<object>> Put<T>(string url, T data);
        Task<HttpResponseWrapper<object>> Delete(string url);
        Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T data, bool includeToken = true);
        Task<HttpResponseWrapper<T>> Get<T>(string url, bool includeToken = true);
    }
}
