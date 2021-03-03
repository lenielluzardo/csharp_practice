using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Http_Client
{
   public class HttpClientFactoryInstanceManagementService:IIntegrationService
   {
      private static HttpClient _httpClient = new HttpClient(
         new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip });

      //Define UseHttpClient() in IConfigureServices and reference with Constructor injection.
      private readonly IHttpClientFactory _httpClientFactory;
      //To Create clients use httpClientFactory.CreateClient()

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
         //await TestDisposeHttpClient(_cancellationTokenSource.Token);
         await TestReuseHttpClient(_cancellationTokenSource.Token);
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
     
   }
}
