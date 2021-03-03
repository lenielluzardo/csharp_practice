using Http_Client.CustomClients;
using Movies.Client.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Http_Client
{
   public class HttpClientFactoryInstanceManagementService : IIntegrationService
   {
      private readonly IHttpClientFactory _httpClientFactory;
      private readonly MoviesClient _moviesClient;

      private static HttpClient _httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip });
      //We declare the cancellation token source at the scope of services to access it from different places.
      private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();


      public HttpClientFactoryInstanceManagementService(IHttpClientFactory httpClientFactory, MoviesClient moviesClient)
      {
         _httpClientFactory = httpClientFactory;
         _moviesClient = moviesClient;
         _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
         _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
      }
      public async Task Run()
      {
         //await TestDisposeHttpClient(_cancellationTokenSource.Token);
         //await TestReuseHttpClient(_cancellationTokenSource.Token);
         //await GetMoviesWithHttpClientFromFactory(_cancellationTokenSource.Token);
         //await GetMoviesWithNamedHttpClientFromFactory(_cancellationTokenSource.Token);
         //await GetMoviesWithTypedHttpClientFromFactory(_cancellationTokenSource.Token);
         await GetMoviesViaMoviesClient(_cancellationTokenSource.Token);
      }
      private async Task TestDisposeHttpClient(CancellationToken cancellationToken)
      {
         for (var i = 0; i < 10; ++i)
         {
            using (var httpClient = new HttpClient())
            {
               var request = new HttpRequestMessage(HttpMethod.Get, "https://www.google.com");

               using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
               {
                  var stream = await response.Content.ReadAsStreamAsync();
                  response.EnsureSuccessStatusCode();
                  Console.WriteLine($"Request completed with status code {response.StatusCode}");
               }
            }
         }
      }
      private async Task TestReuseHttpClient(CancellationToken cancellationToken)
      {
         var httpClient = new HttpClient();
         for (var i = 0; i < 10; ++i)
         {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://www.google.com");

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
               var stream = await response.Content.ReadAsStreamAsync();
               response.EnsureSuccessStatusCode();
               Console.WriteLine($"Request completed with status code {response.StatusCode}");
            }
         }
      }
      private async Task GetMoviesWithHttpClientFromFactory(CancellationToken cancellationToken)
      {
         var httpClient = _httpClientFactory.CreateClient();

         var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:57863/api/movies");
         request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

         using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
         {
            var stream = await response.Content.ReadAsStreamAsync();
            response.EnsureSuccessStatusCode();
            var movies = stream.ReadAndDeserializeFromJson<List<Movie>>();
         }
      }
      private async Task GetMoviesWithNamedHttpClientFromFactory(CancellationToken cancellationToken)
      {
         var httpClient = _httpClientFactory.CreateClient("MoviesClient");

         var request = new HttpRequestMessage(HttpMethod.Get, "api/movies");
         request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
         request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

         using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
         {
            var stream = await response.Content.ReadAsStreamAsync();
            response.EnsureSuccessStatusCode();
            var movies = stream.ReadAndDeserializeFromJson<List<Movie>>();
         }
      }
      
      //##This implementation was moved to the Custom Client Type Class.##
      //private async Task GetMoviesWithTypedHttpClientFromFactory(CancellationToken cancellationToken)
      //{
      //   var request = new HttpRequestMessage(HttpMethod.Get, "api/movies");
      //   request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      //   request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

      //   using (var response = await _moviesClient.Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
      //   {
      //      var stream = await response.Content.ReadAsStreamAsync();
      //      response.EnsureSuccessStatusCode();
      //      var movies = stream.ReadAndDeserializeFromJson<List<Movie>>();
      //   }
      //}
      private async Task GetMoviesViaMoviesClient(CancellationToken cancellationToken)
      {
         var movies = await _moviesClient.GetMovies(cancellationToken);
      }
   }
}
