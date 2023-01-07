using System.Text;
using System.Text.Json;

namespace BlazorMovies.Client.Helpers
{
    public class HttpService : IHttpService
    {
        private readonly HttpClientWithToken httpClientWithToken;
        private readonly HttpClientWithoutToken httpClientWithoutToken;

        private JsonSerializerOptions defaultJsonSerializerOptions => new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

        public HttpService(HttpClientWithToken httpClientWithToken, HttpClientWithoutToken httpClientWithoutToken)
        { 
            this.httpClientWithToken = httpClientWithToken;
            this.httpClientWithoutToken = httpClientWithoutToken;
        }

        private HttpClient GetHttpClient(bool includeToken = true)
        {
            if (includeToken)
                return this.httpClientWithToken.HttpClient;
            else
                return this.httpClientWithoutToken.HttpClient;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data">object to sent to the application</param>
        /// <returns></returns>
        public async Task<HttpResponseWrapper<object>> Post<T>(string url, T data)
        {
            var dataJson = JsonSerializer.Serialize(data);
            var stringContent = new StringContent(dataJson, Encoding.UTF8, "application/json");

            var httpClient = this.GetHttpClient();
            var httpResponse = await httpClient.PostAsync(url, stringContent);
            var wrapper = new HttpResponseWrapper<object>(null, httpResponse.IsSuccessStatusCode, httpResponse);
            return wrapper; 
        }

        public async Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T data, bool includeToken = true)
        {
            var dataJson = JsonSerializer.Serialize(data);
            var stringContent = new StringContent(dataJson, Encoding.UTF8, "application/json");
            var httpClient = this.GetHttpClient(includeToken);
            var httpResponse = await httpClient.PostAsync(url, stringContent);

            if (httpResponse.IsSuccessStatusCode)
            {
                var reponseDeserialized = await Deserialize<TResponse>(httpResponse, defaultJsonSerializerOptions);
                return new HttpResponseWrapper<TResponse>(reponseDeserialized, httpResponse.IsSuccessStatusCode, httpResponse);
            }
            else
            {
                return new HttpResponseWrapper<TResponse>(default, httpResponse.IsSuccessStatusCode, httpResponse);
            }
        }

        private async Task<T> Deserialize<T>(HttpResponseMessage httpResponse, JsonSerializerOptions options)
        {
            var responseString = await httpResponse.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseString, options);
        }

        public async Task<HttpResponseWrapper<T>> Get<T>(string url, bool includeToken = true)
        {
            var httpClient = this.GetHttpClient(includeToken);
            var httpResponse = await httpClient.GetAsync(url);
            if (httpResponse.IsSuccessStatusCode)
            {
                var response = await Deserialize<T>(httpResponse, defaultJsonSerializerOptions);
                return new HttpResponseWrapper<T>(response, httpResponse.IsSuccessStatusCode, httpResponse);
            }
            else
            {
                return new HttpResponseWrapper<T>(default, httpResponse.IsSuccessStatusCode, httpResponse);
            }
        }

        //to update a resource
        public async Task<HttpResponseWrapper<object>> Put<T>(string url, T data)
        {
            var dataJson = JsonSerializer.Serialize(data);
            var stringContent = new StringContent(dataJson, Encoding.UTF8, "application/json");

            var httpClient = this.GetHttpClient();
            var httpResponse = await httpClient.PutAsync(url, stringContent);
            var wrapper = new HttpResponseWrapper<object>(null, httpResponse.IsSuccessStatusCode, httpResponse);
            return wrapper;
        }

        public async Task<HttpResponseWrapper<object>> Delete(string url)
        {
            var httpClient = this.GetHttpClient();
            var httpResponse = await httpClient.DeleteAsync(url);
            return new HttpResponseWrapper<object>(null, httpResponse.IsSuccessStatusCode, httpResponse);
        }
    }
}

