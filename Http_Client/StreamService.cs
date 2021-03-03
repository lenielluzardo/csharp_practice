using Movies.Client.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Http_Client
{
   /// <summary>
   /// Provides a set of Http Request Method that improve the performance using Streams.
   /// </summary>
   public class StreamService : IIntegrationService
   {
      //To be able to use Gzip Compression with need to instantiate our HttpClient with an HttpClientHandler specifiying the AutomaticDecompression for Gzip.
      //The frameworks has built in the functionality but we need to explicitly specify it.
      private static HttpClient _httpClient = new HttpClient(
         new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip });
      public StreamService()
      {
         _httpClient.BaseAddress = new Uri("http://localhost:57863/");
         _httpClient.Timeout = new TimeSpan(0, 0, 30);
         _httpClient.DefaultRequestHeaders.Clear(); //Clear it before setup in case other code has setted.
         _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
         _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
      }
      public async Task Run()
      {
         //await GetPosterWithStream();
         //await GetPosterWithStreamAndCompletionMode();
         await GetPosterWithGzipCompressionStream();
         //await CreateResourceWithStream();
         //await CreateAndGetResourceWithStream();

         //******Testing Methods*******
         //await TestGetPosterWithoutStream();
         //await TestGetPosterWithStream();
         //await TestGetPosterWithStreamAndCompletionMode();
         //await TestCreateResourceWithoutStream();
         //await TestCreateResourceWithStream();
         //await TestCreateAndGetResourceWithoutStream();
      }
      private async Task GetPosterWithStream()
      {
         var request = new HttpRequestMessage(HttpMethod.Get, $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
         request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

         using (var response = await _httpClient.SendAsync(request))
         {
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();

            using (StreamReader streamReader = new StreamReader(stream))
            {
               using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
               {
                  var jsonSerializer = new JsonSerializer();
                  var poster = jsonSerializer.Deserialize<Poster>(jsonTextReader);
                  //do something with poster.
               }
            }
         }
      }
      private async Task GetPosterWithStreamAndCompletionMode()
      {
         var request = new HttpRequestMessage(HttpMethod.Get, $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
         request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

         using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
         {
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();

            //Use the extension method for read and deserialize the stream from request.
            var poster = stream.ReadAndDeserializeFromJson<Poster>();
         }
      }
      private async Task GetPosterWithoutStream()
      {
         var request = new HttpRequestMessage(HttpMethod.Get, $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
         request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

         var response = await _httpClient.SendAsync(request);
         response.EnsureSuccessStatusCode();

         var content = await response.Content.ReadAsStringAsync();
         var posters = JsonConvert.DeserializeObject<Poster>(content);

      }
      private async Task GetPosterWithGzipCompressionStream()
      {
         var request = new HttpRequestMessage(HttpMethod.Get, $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
         request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
         request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

         using (var response = await _httpClient.SendAsync(request))
         {
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var poster = stream.ReadAndDeserializeFromJson<Poster>();
         }
      }

      private async Task CreateResourceWithoutStream()
      {
         //Generate a movie poster of 500KB
         var random = new Random();
         var generatedBytes = new byte[524288];
         random.NextBytes(generatedBytes);

         var posterForCreation = new PosterForCreation
         {
            Name = "A new poster for the Big Lebowski",
            Bytes = generatedBytes
         };

         var contentToCreate = JsonConvert.SerializeObject(posterForCreation);

         var request = new HttpRequestMessage(HttpMethod.Post, "api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/");
         request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
         request.Content = new StringContent(contentToCreate);
         request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

         var response = await _httpClient.SendAsync(request);
         response.EnsureSuccessStatusCode();

         var createdContent = await response.Content.ReadAsStringAsync();
         var posterCreated = JsonConvert.DeserializeObject<Poster>(createdContent);
      }
      private async Task CreateResourceWithStream()
      {
         //Generate a movie poster of 500KB
         var random = new Random();
         var generatedBytes = new byte[524288];
         random.NextBytes(generatedBytes);

         var posterForCreation = new PosterForCreation
         {
            Name = "A new poster for the Big Lebowski",
            Bytes = generatedBytes
         };

         var memoryContentStream = new MemoryStream();
         
         //METHOD 1: WITHOUT EXTENSION METHOD.
         
         //using(var streamWriter = new StreamWriter(memoryContentStream, new UTF32Encoding(), 1024, true))
         //{
         //   using (var jsonTextWriter = new JsonTextWriter(streamWriter))
         //   {
         //      var jsonSerializer = new JsonSerializer();
         //      jsonSerializer.Serialize(jsonTextWriter, posterForCreation);
         //      jsonTextWriter.Flush();
         //   }
         //}

         //METHOD 2: With Extension Method.
         memoryContentStream.SerializeJsonAndWrite(posterForCreation);

         memoryContentStream.Seek(0, SeekOrigin.Begin);
         using(var request = new HttpRequestMessage(HttpMethod.Post, "api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/"))
         {
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using(var streamContent = new StreamContent(memoryContentStream))
            {
               request.Content = streamContent;
               request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

               var response = await _httpClient.SendAsync(request);
               response.EnsureSuccessStatusCode();

               var createContent = await response.Content.ReadAsStringAsync();
               var createdPoster = JsonConvert.DeserializeObject<Poster>(createContent);
            }
         }
      }
      private async Task CreateAndGetResourceWithStream()
      {
         //Generate a movie poster of 500KB
         var random = new Random();
         var generatedBytes = new byte[524288];
         random.NextBytes(generatedBytes);

         var posterForCreation = new PosterForCreation
         {
            Name = "A new poster for the Big Lebowski",
            Bytes = generatedBytes
         };

         var memoryContentStream = new MemoryStream();

         memoryContentStream.SerializeJsonAndWrite(posterForCreation);

         memoryContentStream.Seek(0, SeekOrigin.Begin);
         using (var request = new HttpRequestMessage(HttpMethod.Post, "api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/"))
         {
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var streamContent = new StreamContent(memoryContentStream))
            {
               request.Content = streamContent;
               request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

               using(var response = await _httpClient.SendAsync(request))
               {
                  response.EnsureSuccessStatusCode();

                  var createdContent = await response.Content.ReadAsStreamAsync();
                  var createdPoster = createdContent.ReadAndDeserializeFromJson<Poster>();
               }
            }
         }
      }
      
      #region Test Performance Methods
      public async Task TestGetPosterWithoutStream()
      {
         //warmup
         await GetPosterWithoutStream();

         var stopWatch = Stopwatch.StartNew();

         for(int i = 0; i < 200; i++)
         {
            await GetPosterWithoutStream();
         }

         stopWatch.Stop();

         Console.WriteLine("Elapsed milliseconds without stream: {0}, averaging {1} milliseconds/request",
                           stopWatch.ElapsedMilliseconds, stopWatch.ElapsedMilliseconds / 200);
      }
      public async Task TestGetPosterWithStream()
      {
         //warmup
         await GetPosterWithStream();

         var stopWatch = Stopwatch.StartNew();

         for (int i = 0; i < 200; i++)
         {
            await GetPosterWithStream();
         }

         stopWatch.Stop();

         Console.WriteLine("Elapsed milliseconds with stream: {0}, averaging {1} milliseconds/request",
                           stopWatch.ElapsedMilliseconds, stopWatch.ElapsedMilliseconds / 200);
      }
      public async Task TestGetPosterWithStreamAndCompletionMode()
      {
         //warmup
         await GetPosterWithStreamAndCompletionMode();

         var stopWatch = Stopwatch.StartNew();

         for (int i = 0; i < 200; i++)
         {
            await GetPosterWithStreamAndCompletionMode();
         }

         stopWatch.Stop();

         Console.WriteLine("Elapsed milliseconds with stream and completion mode: {0}, averaging {1} milliseconds/request",
                           stopWatch.ElapsedMilliseconds, stopWatch.ElapsedMilliseconds / 200);
      }
      public async Task TestCreateResourceWithoutStream()
      {
         //warmup
         await CreateResourceWithoutStream();

         var stopWatch = Stopwatch.StartNew();

         for (int i = 0; i < 200; i++)
         {
            await CreateResourceWithoutStream();
         }

         stopWatch.Stop();

         Console.WriteLine("Elapsed milliseconds creating without stream: {0}, averaging {1} milliseconds/request",
                           stopWatch.ElapsedMilliseconds, stopWatch.ElapsedMilliseconds / 200);
      }
      public async Task TestCreateResourceWithStream()
      {
         //warmup
         await CreateResourceWithStream();

         var stopWatch = Stopwatch.StartNew();

         for (int i = 0; i < 200; i++)
         {
            await CreateResourceWithStream();
         }

         stopWatch.Stop();

         Console.WriteLine("Elapsed milliseconds creating with stream and reading string: {0}, averaging {1} milliseconds/request",
                           stopWatch.ElapsedMilliseconds, stopWatch.ElapsedMilliseconds / 200);
      }
      public async Task TestCreateAndGetResourceWithoutStream()
      {
         //warmup
         await CreateAndGetResourceWithStream();

         var stopWatch = Stopwatch.StartNew();

         for (int i = 0; i < 200; i++)
         {
            await CreateAndGetResourceWithStream();
         }

         stopWatch.Stop();

         Console.WriteLine("Elapsed milliseconds creating and reading with stream: {0}, averaging {1} milliseconds/request",
                           stopWatch.ElapsedMilliseconds, stopWatch.ElapsedMilliseconds / 200);
      }

      #endregion
   }
}
