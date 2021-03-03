using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Marvin.StreamExtensions;
using ZZ_Common.Interfaces;
using ZZ_Common.Models;

namespace Http_Client
{
   public class DealingWithErrorsAndFaultsService : IHttpClientService
   {
      private readonly IHttpClientFactory _httpClientFactory;
      private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
      public DealingWithErrorsAndFaultsService(IHttpClientFactory httpClientFactory)
      {
         _httpClientFactory = httpClientFactory;
      }
      public async Task Run()
      {
         //await GetMovieAndDealWithInvalidResponses(_cancellationTokenSource.Token);
         await PostMovieAndHandleValidationErrors(_cancellationTokenSource.Token);
      }

      private async Task GetMovieAndDealWithInvalidResponses(CancellationToken cancellationToken)
      {
         var httpClient = _httpClientFactory.CreateClient("MoviesClient");

         var request = new HttpRequestMessage(HttpMethod.Get, "api/movies/f9a16fee-4c49-41bb-87a1-bbaad0cd117a");
         request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
         request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

         using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
         {
            if (!response.IsSuccessStatusCode)
            {
               if (response.StatusCode == HttpStatusCode.NotFound)
               {
                  Console.WriteLine("The requested movie cannot be found");
                  return;
               }
            }

            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var movie = stream.ReadAndDeserializeFromJson<Movie>();
         }
      }
      private async Task PostMovieAndHandleValidationErrors(CancellationToken cancellationToken)
      {
         var httpClient = _httpClientFactory.CreateClient("MoviesClient");

         var movieForCreation = new MovieForCreation
         {
            Title = "Pulp Fiction",
            Description = "Too short",
            DirectorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
            ReleaseDate = new DateTimeOffset(new DateTime(1992, 9, 2)),
            Genre = "Crime, Drama"
         };

         var serializedMovieForCreation = JsonConvert.SerializeObject(movieForCreation);

         using (var request = new HttpRequestMessage(HttpMethod.Post, "api/movies"))
         {
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            request.Content = new StringContent(serializedMovieForCreation);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
               if (!response.IsSuccessStatusCode)
               {
                  if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
                  {
                     var errorStream = await response.Content.ReadAsStreamAsync();
                     var validationErrors = errorStream.ReadAndDeserializeFromJson();
                     Console.WriteLine(validationErrors);
                     return;
                  }
                  else
                  {
                     response.EnsureSuccessStatusCode();

                  }
               }
               var stream = await response.Content.ReadAsStreamAsync();
               var movie = stream.ReadAndDeserializeFromJson<Movie>();
            }
         }
      }
   }
}
