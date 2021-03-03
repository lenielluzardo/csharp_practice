using Movies.Client.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Http_Client
{
   public class TestableClassWithApiAccess
   {
      private HttpClient _httpClient;

      public TestableClassWithApiAccess(HttpClient httpClient)
      {
         _httpClient = httpClient;
      }

      public async Task<Movie> GetMovie(CancellationToken cancellationToken)
      {

         var request = new HttpRequestMessage(HttpMethod.Get, "api/movies/f9a16fee-4c49-41bb-87a1-bbaad0cd117a");
         request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
         request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

         using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
         {
            if (!response.IsSuccessStatusCode)
            {
               if (response.StatusCode == HttpStatusCode.NotFound)
               {
                  Console.WriteLine("The requested movie cannot be found");
                  return null;
               }
               else if(response.StatusCode == HttpStatusCode.Unauthorized)
               {
                  throw new UnauthorizedApiAccessException();
               }
               response.EnsureSuccessStatusCode();
            }
            var stream = await response.Content.ReadAsStreamAsync();
            return stream.ReadAndDeserializeFromJson<Movie>();
         }
      }
   }
}
