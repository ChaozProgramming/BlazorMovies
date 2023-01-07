using System.Net.Http;

namespace BlazorMovies.Client.Helpers
{
    public class HttpResponseWrapper<T>
    {
        public HttpResponseWrapper(T reponse, bool succes, HttpResponseMessage httpReponseMessage)
        {
            Response = reponse;
            Succes = succes;
            HttpResponseMessage = httpReponseMessage;
        }

        public bool Succes { get; set; }    
        public T Response { get; set; }
        public HttpResponseMessage HttpResponseMessage { get; set; }

        public async Task<string> GetBody()
        { 
            return await HttpResponseMessage.Content.ReadAsStringAsync();
        }
    }
}
