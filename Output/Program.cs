using Http_Client;
using Microsoft.Extensions.DependencyInjection;
using System;
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

      #region Configuration
      private static void ConfigureServices(IServiceCollection serviceCollection)
      {
         serviceCollection.AddHttpClient();


         //········ CUSTOM SERVICES ·········

         //serviceCollection.AddScoped<IIntegrationService, CRUDService>();
         //serviceCollection.AddScoped<IIntegrationService, PartialUpdateService>();
         //serviceCollection.AddScoped<IIntegrationService, StreamService>();
         //serviceCollection.AddScoped<IIntegrationService, CancellationService>();
         serviceCollection.AddScoped<IIntegrationService, HttpClientFactoryInstanceManagementService>();
         //serviceCollection.AddScoped<IIntegrationService, DealingWithErrorsAndFaultsService>();
         //serviceCollection.AddScoped<IIntegrationService, HttpHandlersService>();     
      }
      #endregion 
   }
}
