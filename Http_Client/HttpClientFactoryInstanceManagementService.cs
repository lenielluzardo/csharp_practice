using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Http_Client
{
   public class HttpClientFactoryInstanceManagementService
   {
      private static HttpClient _httpClient = new HttpClient(
         new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip });

      //We declare the cancellation token source at the scope of services to access it from different places.
      private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
      public HttpClientFactoryInstanceManagementService()
      {
         _httpClient.BaseAddress = new Uri("http://localhost:57863/");
         _httpClient.Timeout = new TimeSpan(0, 0, 0, 0, 500);
         _httpClient.DefaultRequestHeaders.Clear(); //Clear it before setup in case other code has setted.
         _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
         _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
      }

      public async Task Run()
      {
      }
      private async Task TestDisposeHttpClient(CancelationToken cancellationToken)
   }
}
