using ZZ_Common.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Http_Client.Customs
{
   public class MoviesClient
   {
      private HttpClient _client { get; }

      public MoviesClient(HttpClient client)
      {
         _client = client;
         _client.BaseAddress = new Uri("http://localhost:57863/");
         _client.Timeout = new TimeSpan(0, 0, 0, 30, 500);
         _client.DefaultRequestHeaders.Clear();
      }
      
      public async Task<List<Movie>> GetMovies(CancellationToken cancellationToken)
      {
         var request = new HttpRequestMessage(HttpMethod.Get, "api/movies");
         request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
         request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

         using (var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
         {
            var stream = await response.Content.ReadAsStreamAsync();
            response.EnsureSuccessStatusCode();
            var movies = stream.ReadAndDeserializeFromJson<List<Movie>>();
            
            return movies;
         }
      }
   }
}
