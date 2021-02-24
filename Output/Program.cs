using Output.Algorithms_DataStructure;
using Output.Files_Streams;
using System;
using System.Collections.Generic;
using static System.Console;
using System.IO;

namespace Output
{
   /// <summary>
   /// Shared console program for all exercises in the solution.
   /// </summary>
   class Program
   {
      static void Main(string[] args)
      {
         //PalindromeExe.RunExercise();
         //FizzBuzzExe.RunExercise();
         //SinglyLinkedList.Exe();
         //DoublyLinkedList<int>.Exe();

         ValidateConsoleArgs(args);

         Console.Read();
      }
      /// <summary>
      /// Checks if the arguments passed to the console are supported.
      /// </summary>
      /// <param name="args">Console 'flags'</param>
      static void ValidateConsoleArgs(string[] args)
      {
         var command = args[0];

         if(command == "--file")
         {
            var filePath = args[1];
            WriteLine($"Single file {filePath} selected");
            FileProcessor.ProcessSingleFile(filePath);
         }
         else if (command == "--dir")
         {
            var directoryPath = args[1];
            var fileType = args[2];
            WriteLine($"Directory {directoryPath} selected for {fileType} files");
            FileProcessor.ProcessDirectory(directoryPath, fileType);
         }
         else
         {
            WriteLine("Invalid command line options");
         }

         WriteLine("Press enter to quit.");
         ReadLine();
      }
      
   }
}
