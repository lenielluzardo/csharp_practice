using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZZ_Common.Interfaces;


namespace Http_Client
{
   public class HttpHandlersService : IHttpClientService
   {
      private readonly IHttpClientFactory _httpClientFactory;
      private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
      public HttpHandlersService(IHttpClientFactory httpClientFactory)
      {
         _httpClientFactory = httpClientFactory;
      }

      public async Task Run()
      {
         await GetMoviesWithRetryPolicy(_cancellationTokenSource.Token);
      }

      private async Task GetMoviesWithRetryPolicy(CancellationToken cancellationToken)
      {
         var httpClient = _httpClientFactory.CreateClient("MoviesClient");

         var request = new HttpRequestMessage(HttpMethod.Get, "api/movies/bb6a100a-053f-4bf8-b271-60ce3aae6eb5");
         request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
         request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

         using(var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
         {
            if (!response.IsSuccessStatusCode)
            {
               if(response.StatusCode == HttpStatusCode.NotFound)
               {
                  Console.WriteLine("The requested movie cannot be found.");
                  return;
               }
            }
         }
      }
   }
}
