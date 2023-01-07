using BlazorMovies.Client;
using BlazorMovies.Client.Helpers;
using BlazorMovies.Client.Repository;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//Http Client by Name or  By Type
//Http Client with and without token.
//Problem was that the application throwed a 403 error when using the application with no token
//for example anonymous users
/*
builder.Services.AddHttpClient("BlazorMovies.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorMovies.ServerAPI"));
*/
builder.Services.AddHttpClient<HttpClientWithToken>(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>(); //Add the JWT Token into the client

builder.Services.AddHttpClient<HttpClientWithoutToken>(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

builder.Services.AddApiAuthorization();

ConfigureServices(builder.Services);

await builder.Build().RunAsync();

static void ConfigureServices(IServiceCollection services)
{
    services.AddTransient<IRepository, RepositoryInMemory>();
    services.AddScoped<IHttpService, HttpService>();
    services.AddScoped<IGenreRepository, GenreRepository>();
    services.AddScoped<IPersonRepository, PersonRepository>();
    services.AddScoped<IMovieRepository, MovieRepository>();
    services.AddScoped<IRatingRepository, RatingRepository>();
    services.AddScoped<IDisplayMessage, DisplayMessage>();
    services.AddScoped<IUserRepository, UserRepository>();



}