using Files_Streams;
using Http_Client;
using System;
using System.Diagnostics;
using static System.Console;

namespace AA_Output
{
   /// <summary>
   /// Shared console program for all exercises in the solution.
   /// </summary>
   class Program
   {
      static void Main(string[] args)
      {
         try
         {
            TextWriterTraceListener tr1 = new TextWriterTraceListener(Console.Out);
            Trace.Listeners.Add(tr1);
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
            var client = new HttpClientFactoryInstanceManagementService();
            client.Run();
            #endregion

            Trace.Flush();
            
         }
         catch(Exception ex)
         {
            WriteLine(ex.ToString());
         }
        
         ReadLine();
      }
   }
}
