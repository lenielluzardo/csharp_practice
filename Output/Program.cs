using Http_Client;
using Http_Client.CustomClients;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
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
            #region Random Excercises
            //PalindromeExe.RunExercise();
            //FizzBuzzExe.RunExercise();
            //SinglyLinkedList.Exe();
            //DoublyLinkedList<int>.Exe();
            #endregion

            #region .Net Base Class Library
            //## Working with Files and Streams
            //FileProcessor.ValidateConsoleArgs(args);

            //## Working with Http Client
            await serviceProvider.GetService<IIntegrationService>().Run();

            #endregion
            
         }
         catch(Exception ex)
         {
            WriteLine(ex.ToString());
         }

         ReadKey();

      }

      private static void ConfigureServices(IServiceCollection serviceCollection)
      {
         #region Http Client Configuration
         //Register custom clients with strings as Key for its instantiation/configuration.
         serviceCollection.AddHttpClient("MoviesClient", client =>
         {
            client.BaseAddress = new Uri("http://localhost:57863/");
            client.Timeout = new TimeSpan(0, 0, 0, 30, 500);
            client.DefaultRequestHeaders.Clear(); //Clear it before setup in case other code has setted.
         })
         .ConfigurePrimaryHttpMessageHandler(handler => new HttpClientHandler()
         {
            AutomaticDecompression = System.Net.DecompressionMethods.GZip
         });

         //Register custom clients with custom types for its instantiation/configuration.
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

         //········ CUSTOM SERVICES ·········

         //serviceCollection.AddScoped<IIntegrationService, CRUDService>();
         //serviceCollection.AddScoped<IIntegrationService, PartialUpdateService>();
         //serviceCollection.AddScoped<IIntegrationService, StreamService>();
         //serviceCollection.AddScoped<IIntegrationService, CancellationService>();
         //serviceCollection.AddScoped<IIntegrationService, HttpClientFactoryInstanceManagementService>();
         serviceCollection.AddScoped<IIntegrationService, DealingWithErrorsAndFaultsService>();
         //serviceCollection.AddScoped<IIntegrationService, HttpHandlersService>();   

         #endregion

      }
   }
}
