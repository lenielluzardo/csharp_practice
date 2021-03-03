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
   public class CancellationService : IIntegrationService
   {
      private static HttpClient _httpClient = new HttpClient(
         new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip });

      //We declare the cancellation token source at the scope of services to access it from different places.
      private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
      public CancellationService()
      {
         _httpClient.BaseAddress = new Uri("http://localhost:57863/");
         _httpClient.Timeout = new TimeSpan(0,0,0,0,500);
         _httpClient.DefaultRequestHeaders.Clear(); //Clear it before setup in case other code has setted.
         _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
         _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
      }

      public async Task Run()
      {
         //_cancellationTokenSource.CancelAfter(500);
         //await GetTrailerAndCancelAsync(_cancellationTokenSource.Token);
         await GetTrailerAndHandleTimeout();
      }

      private async Task GetTrailerAndCancelAsync(CancellationToken cancellationToken)
      {
         var request = new HttpRequestMessage(HttpMethod.Get, $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/trailers/{Guid.NewGuid()}");

         request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
         request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));


         try
         {
            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
               response.EnsureSuccessStatusCode();

               var stream = await response.Content.ReadAsStreamAsync();
               var trailer = stream.ReadAndDeserializeFromJson<Trailer>();

            }
         }
         catch (OperationCanceledException ex) 
         {
            Console.WriteLine($"An Operation was cancelled producing an error of: {ex.Message}");
         }
      }
      private async Task GetTrailerAndHandleTimeout()
      {
         var request = new HttpRequestMessage(HttpMethod.Get, $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/trailers/{Guid.NewGuid()}");

         request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
         request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

         try
         {
            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
            {
               var stream = await response.Content.ReadAsStreamAsync();
               
               response.EnsureSuccessStatusCode();
               var trailer = stream.ReadAndDeserializeFromJson<Trailer>();
            }
         }
         catch (OperationCanceledException ex)
         {
            Console.WriteLine($"An Operation was cancelled producing an error of: {ex.Message}");
         }
      }
   }
}
