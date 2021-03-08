using Files_Streams;
using Files_Streams.Algorithms_DataStructure;
using Http_Client;
using Http_Client.Customs;
using Http_Client.Handlers;
using Json;
using Microsoft.Extensions.DependencyInjection;
using Random_Exercises;
using Regular_Expressions;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using ZZ_Common.Interfaces;
using static System.Console;

namespace AA_Output
{
   /// <summary>
   /// Shared console program for all exercises in the solution.
   /// </summary>
   class Program
   {
      static async Task Main(string[] args)
      {
         var serviceCollection = new ServiceCollection();
         ConfigureServices(serviceCollection);

         var serviceProvider = serviceCollection.BuildServiceProvider();

         try
         {
            serviceProvider.GetService<IJsonProcessorService>().Run();
         }
         catch (Exception ex)
         {
            WriteLine(ex.ToString());
         }

         ReadKey();
      }

      private static void ConfigureServices(IServiceCollection serviceCollection)
      {
         #region Random Excercises
         serviceCollection.AddScoped<IRandomExercisesService, PalindromeExe>();
         serviceCollection.AddScoped<IRandomExercisesService, FizzBuzzExe>();
         #endregion

         #region Algorithm And Data Structures
         //serviceCollection.AddScoped<IAlgorithDataStructureService, SinglyLinkedList>();
         //serviceCollection.AddScoped<IAlgorithDataStructureService, DoublyLinkedList>();
         #endregion

         #region File Processor Configuration
         serviceCollection.AddScoped<IFileProcessorService, FileProcessor>();
         #endregion

         #region Http Client Configuration

         //Register custom clients with strings as Key for its instantiation/ configuration.
         serviceCollection.AddHttpClient("MoviesClient", client =>
         {
            client.BaseAddress = new Uri("http://localhost:57863/");
            client.Timeout = new TimeSpan(0, 0, 0, 30, 500);
            client.DefaultRequestHeaders.Clear(); //Clear it before setup in case other code has setted.
         })
         .AddHttpMessageHandler(handler => new TimeOutDelegatingHandler(TimeSpan.FromSeconds(20)))
         .AddHttpMessageHandler(handler => new RetryPolicyDelegatingHandler(3))
         .ConfigurePrimaryHttpMessageHandler(handler => new HttpClientHandler()
         {
            AutomaticDecompression = System.Net.DecompressionMethods.GZip
         });

         //Register custom clients with custom types for its instantiation/ configuration.
         serviceCollection.AddHttpClient<MoviesClient>()

         // ··········· This configuration can be achieved within the class.············
         //(client => 
         //{
         //   client.BaseAddress = new Uri("http://localhost:57863/");    
         //   client.Timeout = new TimeSpan(0, 0, 0, 30, 500);
         //   client.DefaultRequestHeaders.Clear();
         //})
         // ·············································································
         .ConfigurePrimaryHttpMessageHandler(handler => new HttpClientHandler()
         {
            AutomaticDecompression = System.Net.DecompressionMethods.GZip
         });

         serviceCollection.AddScoped<IHttpClientService, CRUDService>();
         //serviceCollection.AddScoped<IHttpClientService, StreamService>();
         //serviceCollection.AddScoped<IHttpClientService, CancellationService>();
         //serviceCollection.AddScoped<IHttpClientService, HttpClientFactoryInstanceManagementService>();
         //serviceCollection.AddScoped<IHttpClientService, DealingWithErrorsAndFaultsService>();
         //serviceCollection.AddScoped<IHttpClientService, HttpHandlersService>();

         #endregion

         #region Regular Expressions
         //serviceCollection.AddScoped<IRegexService, RegularExpressionService>();
         #endregion

         #region Json Processor
         serviceCollection.AddScoped<IJsonProcessorService, JsonProcessorService>();
         #endregion
      }
   }
}
