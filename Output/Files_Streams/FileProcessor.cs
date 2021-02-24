using System;
using System.Collections.Generic;
using System.Text;
using static System.Console;
using System.IO;

namespace Output.Files_Streams
{
   /// <summary>
   /// Custom input files processor. 
   /// </summary>
   public class FileProcessor
   {
      private static readonly string BackupDirectoryName = "backup";
      private static readonly string InProgressDirectoryName = "processing";
      private static readonly string CompletedDirectoryName = "completed";
      public FileProcessor(string filePath)
      {
         InputFilePath = filePath;
      }

      public string InputFilePath { get; }

      public void Process()
      {
         WriteLine($"Begin process of {InputFilePath}");

         if (!File.Exists(InputFilePath))
         {
            WriteLine($"ERROR: file {InputFilePath} does not exist.");
            return;
         }

         string rootDirectoryPath = new DirectoryInfo(InputFilePath).Parent.Parent.FullName;
         WriteLine($"Root data path is {rootDirectoryPath}");
         
         string inputFileDirectoryPath = Path.GetDirectoryName(InputFilePath);
         string backupDirectoryPath = Path.Combine(rootDirectoryPath, BackupDirectoryName);

         //if (!Directory.Exists(backupDirectoryPath))
         //{
            WriteLine($"Creating {backupDirectoryPath}");
            Directory.CreateDirectory(backupDirectoryPath);
         //}

         //Copy file to backup dir
         string inputFileName = Path.GetFileName(InputFilePath);
         string backupFilePath = Path.Combine(backupDirectoryPath, inputFileName);
         WriteLine($"Copying {InputFilePath} to {backupFilePath}");

         File.Copy(InputFilePath, backupFilePath, true);

         //Move to in progress dir
         Directory.CreateDirectory(Path.Combine(rootDirectoryPath, InProgressDirectoryName));
         
         var inProgressFilePath = Path.Combine(rootDirectoryPath, InProgressDirectoryName, inputFileName);

         if (File.Exists(inProgressFilePath))
         {
            WriteLine($"ERROR: a file with the name {inProgressFilePath} is already being processed");
            return;
         }

         WriteLine($"Moving {InputFilePath} to {inProgressFilePath}");
         File.Move(InputFilePath, inProgressFilePath);

         //Determine type of file
         string extension = Path.GetExtension(InputFilePath);
         switch (extension)
         {
            case ".txt":
               ProcessTextFile(inProgressFilePath);
               break;
            default:
               WriteLine($"{extension} is an unsupported file type.");
               break;

         }

         string completedDirectoryPath = Path.Combine(rootDirectoryPath, CompletedDirectoryName);
         Directory.CreateDirectory(completedDirectoryPath);

         WriteLine($"Moving {inProgressFilePath} to {completedDirectoryPath}");

         string completedFileName = $"{Path.GetFileNameWithoutExtension(InputFilePath)}-{Guid.NewGuid()}{extension}";
         string completedFilePath = Path.Combine(completedDirectoryPath, completedFileName);

         File.Move(inProgressFilePath, completedFilePath);

         string inProgressDirectoryPath = Path.GetDirectoryName(inProgressFilePath);
         Directory.Delete(inProgressDirectoryPath, true);
      }

      private void ProcessTextFile(string inProgressFilePath)
      {
         WriteLine($"Processing text file {inProgressFilePath}");
         
         //TODO: Read in process
      }

      /// <summary>
      /// Uses FileProcessor class to work with input files base on its path.
      /// </summary>
      /// <param name="filePath"></param>
      public static void ProcessSingleFile(string filePath)
      {
         var fileProcessor = new
            FileProcessor(filePath);
         fileProcessor.Process();
      }
      public static void ProcessDirectory(string directoryPath, string fileType)
      {
         //var allFiles = Directory.GetFiles(directoryPath);

         switch (fileType)
         {
            case "TEXT":
               string[] textFiles = Directory.GetFiles(directoryPath, "*.txt");
               foreach(var textFilePath in textFiles)
               {
                  var fileProcessor = new FileProcessor(textFilePath);
                  fileProcessor.Process();
               }
               break;
            default:
               WriteLine($"ERROR: {fileType} is not supported");
               return;
         }

      }
   }
}
