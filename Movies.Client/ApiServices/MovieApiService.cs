using IdentityModel.Client;
using Movies.Client.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Movies.Client.ApiServices
{
    public class MovieApiService : IMovieApiService
    {
        public Task<Movie> CreateMovie(Movie movie)
        {
            throw new NotImplementedException();
        }

        public Task DeleteMovie(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Movie> GetMovie(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Movie>> GetMovies()
        {
            //1. "retrieve" our api credentials. This must be registered on IS
            var apiClientCredentials = new ClientCredentialsTokenRequest
            {
                Address = "https://localhost:5005/connect/token",

                ClientId = "movieClient",
                ClientSecret = "secret",

                Scope = "movieAPI"
            };

            //creates a new HttpClient to talk to our IS
            var client = new HttpClient();

            //just checks if we can reach the Discovery document
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5005");
            if(disco.IsError) 
                return null;

            //2. Authenticates and get an access token from IS
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(apiClientCredentials);
            if (tokenResponse.IsError)
                return null;

            //3. Send request to protected API
            //another client for talking with protected API
            var apiClient = new HttpClient();

            //4. Set the access_token in the request authorization: Bearer <Token>
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            //5. Send a request to our protected API
            var response = await apiClient.GetAsync("https://localhost:5001/api/movies");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            //6. Deserialize obj to movielist
            List<Movie> movieList = JsonConvert.DeserializeObject<List<Movie>>(content);
            return movieList;
        }

        public Task<Movie> UpdateMovie(Movie movie)
        {
            throw new NotImplementedException();
        }
    }
}
