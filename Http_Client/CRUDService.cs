using Microsoft.AspNetCore.JsonPatch;
using Movies.Client.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Http_Client
{
   public class CRUDService
   {
      private static HttpClient _httpClient = new HttpClient();
      public CRUDService()
      {
         _httpClient.BaseAddress = new Uri("http://localhost:57863");
         _httpClient.Timeout = new TimeSpan(0, 0, 30);
         _httpClient.DefaultRequestHeaders.Clear(); //Clear it before setup in case other code has setted.
         _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
         _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
      }
      public async Task Run()
      {
         //await GetResource();
         //await GetResourceThroughHttpRequestMessage();
         //await CreateResource();
         //await UpdateResource();
         //await DeleteResource();
         await PatchResource();
      }

      /// <summary>
      /// Sends a request with a default Accept Header Content Type.
      /// </summary>
      private async Task GetResource()
      {
         var response = await _httpClient.GetAsync("api/movies");
         response.EnsureSuccessStatusCode();

         // We need to explicitly call the read content method. 
         var content = await response.Content.ReadAsStringAsync();
         var movies = new List<Movie>();

         if (response.Content.Headers.ContentType.MediaType == "application/json")
         {
            movies = JsonConvert.DeserializeObject<List<Movie>>(content);
         }
         else if (response.Content.Headers.ContentType.MediaType == "application/xml")
         {
            var serializer = new XmlSerializer(typeof(List<Movie>));
            movies = (List<Movie>)serializer.Deserialize(new StringReader(content));
         }
      }
      ///<summary>
      ///Sends a request with a non-default Accept Header Content Type.
      ///</summary>
      private async Task GetResourceThroughHttpRequestMessage()
      {
         var request = new HttpRequestMessage(HttpMethod.Get, "api/movies");
         request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

         //With HttpRequestMessage with can use any of the HttpClient methods for sending requests.
         var response = await _httpClient.SendAsync(request);

         response.EnsureSuccessStatusCode();

         var content = await response.Content.ReadAsStringAsync();
         var movies = JsonConvert.DeserializeObject<List<Movie>>(content);
      }
      private async Task CreateResource()
      {
         var movieToCreate = new MovieForCreation
         {
            Title = "Reservoir Dogs",
            Description = "After a simple jewerly heist goes terribly wrong, the surviving criminals begin to suspect that one of them is a police informant.",
            DirectorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
            ReleaseDate = new DateTimeOffset(new DateTime(199, 9, 2)),
            Genre = "Crime, Drama"
         };
         
         //METHOD 1: Specifying explicitly each property to be configured.
         var serializedMoviToCreate = JsonConvert.SerializeObject(movieToCreate);

         var request = new HttpRequestMessage(HttpMethod.Post, "api/movies");
         request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

         request.Content = new StringContent(serializedMoviToCreate);
         request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

         var response = await _httpClient.SendAsync(request);

         //METHOD 2: Use a shortcut for HttpRequestMessage creation.
         var shortRequest = _httpClient.PostAsync("uri", new StringContent(
         JsonConvert.SerializeObject(movieToCreate), Encoding.UTF8, "application/json"));

         response.EnsureSuccessStatusCode();

         var content = await response.Content.ReadAsStringAsync();

         var createdMovie = JsonConvert.DeserializeObject<Movie>(content);
      }
      private async Task UpdateResource()
      {
         var movieToUpdate = new MovieForUpdate
         {
            Title = "Pulp Fiction",
            Description = "The movie with Zed.",
            DirectorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
            ReleaseDate = new DateTimeOffset(new DateTime(1992, 9, 2)),
            Genre = "Crime, Drama"
         };

         var serializedMovieToUpdate = JsonConvert.SerializeObject(movieToUpdate);

         var request = new HttpRequestMessage(HttpMethod.Put, "api/movies/5b1c2b4d-48c7-402a-80c3-cc796ad49c6b");
         request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
         request.Content = new StringContent(serializedMovieToUpdate);
         request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

         var response = await _httpClient.SendAsync(request);
         response.EnsureSuccessStatusCode();

         var content = await response.Content.ReadAsStringAsync();
         var updatedMovie = JsonConvert.DeserializeObject<Movie>(content);
      }
      private async Task DeleteResource()
      {
         var request = new HttpRequestMessage(HttpMethod.Delete, "api/movies/5b1c2b4d-48c7-402a-80c3-cc796ad49c6b");
         request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

         var response = await _httpClient.SendAsync(request);
         response.EnsureSuccessStatusCode();

         var content = await response.Content.ReadAsStringAsync();
      }
      private async Task PatchResource()
      {
         var patchDoc = new JsonPatchDocument<MovieForUpdate>();
         
         //We specify the operation that we want to be executed for the patch request.
         patchDoc.Replace(m => m.Title, "Updated Title");
         patchDoc.Remove(m => m.Description);

         var serializedChangeSet = JsonConvert.SerializeObject(patchDoc);

         var request = new HttpRequestMessage(HttpMethod.Patch, "api/movies/3d2880ae-5ba6-417c-845d-f4ebfd4bcac7");
         request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
         request.Content = new StringContent(serializedChangeSet);

         //Instead of using the default Content-Type. We use the 'json-patch+json' Content-Type specification.
         request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json-patch+json");

         var response = await _httpClient.SendAsync(request);
         response.EnsureSuccessStatusCode();

         var content = await response.Content.ReadAsStringAsync();
         var updatedMovie = JsonConvert.DeserializeObject<Movie>(content);

      }
   }
}
