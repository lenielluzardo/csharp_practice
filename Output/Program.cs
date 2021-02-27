using Files_Streams;
using Http_Client;
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
         var client = new CRUDService();
         client.Run();
         #endregion

         ReadLine();
      }
   }
}
