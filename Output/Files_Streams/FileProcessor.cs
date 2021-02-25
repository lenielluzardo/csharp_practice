using System;
using System.Collections.Generic;
using System.Text;
using static System.Console;
using System.IO;
using System.Collections.Concurrent;
using System.Threading;
using System.Runtime.Caching;

namespace Output.Files_Streams
{
   /// <summary>
   /// Process files through a console application.
   /// </summary>
   public class FileProcessor
   {
      private static readonly string BackupDirectoryName = "backup";
      private static readonly string InProgressDirectoryName = "processing";
      private static readonly string CompletedDirectoryName = "completed";

      private static ConcurrentDictionary<string, string> FilesToProcessDictionary = new ConcurrentDictionary<string, string>();
      private static MemoryCache FilesToProcessCache = MemoryCache.Default;
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

         string extension = Path.GetExtension(InputFilePath);

         string completedDirectoryPath = Path.Combine(rootDirectoryPath, CompletedDirectoryName);
         Directory.CreateDirectory(completedDirectoryPath);

         string completedFileName = $"{Path.GetFileNameWithoutExtension(InputFilePath)}-{Guid.NewGuid()}{extension}";
         string completedFilePath = Path.Combine(completedDirectoryPath, completedFileName);

         //Determine type of file
         switch (extension)
         {
            case ".txt":
               var textProcessor = new TextFileProcessor(inProgressFilePath, completedFilePath);
               textProcessor.Process();
               break;
            case ".data":
               var binaryProcessor = new BinaryFileProcessor(inProgressFilePath, completedFilePath);
               binaryProcessor.Process();
               break;
            case ".csv":
               var csvProcessor = new CsvFileProcessor(inProgressFilePath, completedFilePath);
               csvProcessor.Process();
               break;
            default:
               WriteLine($"{extension} is an unsupported file type.");
               break;

         }

        

         WriteLine($"Completed processing of {inProgressFilePath}");

         WriteLine($"Deleting {inProgressFilePath}");
         File.Delete(inProgressFilePath);

         //string inProgressDirectoryPath = Path.GetDirectoryName(inProgressFilePath); // When using cache the process of files is executed in parallel and throws an exception is inProgress folder isn't found.
         //Directory.Delete(inProgressDirectoryPath, true);
      }

      /// <summary>
      /// Uses FileProcessor class to work with input files base on its path.
      /// </summary>
      /// <param name="filePath"></param>
      public static void ProcessSingleFile(string filePath)
      {
         var fileProcessor = new FileProcessor(filePath);
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
      #region Console Arguments Validations
      /// <summary>
      /// Checks if the arguments passed to the console are supported.
      /// </summary>
      /// <param name="args">Console 'flags'</param>
      public static void ValidateConsoleArgs(string[] args)
      {
         var directoryToWatch = args[0];
         if (!Directory.Exists(directoryToWatch))
         {
            WriteLine($"ERROR: {directoryToWatch} does not exist.");
         }
         else
         {
            WriteLine($"Watching directory {directoryToWatch} for changes.");

            ProcessExistingFiles(directoryToWatch);

            using (var inputFileWatcher = new FileSystemWatcher(directoryToWatch))
            //using (var timer = new Timer(ProcessFilesWithConcurrentDictionary, null, 0, 1000))  // Use this for FilesToProcess ConcurrentDictionary
            {
               inputFileWatcher.IncludeSubdirectories = false;
               inputFileWatcher.InternalBufferSize = 32768; // 32KB
               inputFileWatcher.Filter = "*.*";
               inputFileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;

               // Register Event Handlers
               inputFileWatcher.Created += FileCreated;
               inputFileWatcher.Changed += FileChanged;
               inputFileWatcher.Deleted += FileDeleted;
               inputFileWatcher.Renamed += FileRenamed;
               inputFileWatcher.Error += WatcherError;

               inputFileWatcher.EnableRaisingEvents = true;

               WriteLine("Press enter to quit.");
               ReadLine();

            }
         }


      }

      private static void ProcessExistingFiles(string inputDirectory)
      {
         WriteLine($"Checking {inputDirectory} for existing files");

         foreach(var filePath in Directory.EnumerateFiles(inputDirectory))
         {
            WriteLine($"  - Found {filePath}");
            AddToCache(filePath);
         }
         
      }



      /// <summary>
      /// Checks if the arguments passed to the console are supported.
      /// </summary>
      /// <param name="args">Console 'flags'</param>
      public static void ValidateIndividualConsoleArgs(string[] args)
      {
         var command = args[0];


         if (command == "--file")
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
      #endregion

      #region Event Handlers
      private static void WatcherError(object sender, ErrorEventArgs e)
      {
         WriteLine($"ERROR: file system watching may no longer be active: {e.GetException()}");
      }

      private static void FileRenamed(object sender, RenamedEventArgs e)
      {
         WriteLine($"* File renamed: {e.Name} - type: {e.ChangeType}");
      }

      private static void FileDeleted(object sender, FileSystemEventArgs e)
      {
         WriteLine($"* File deleted: {e.Name} - type: {e.ChangeType}");
      }

      private static void FileChanged(object sender, FileSystemEventArgs e)
      {
         WriteLine($"* File changed: {e.Name} - type: {e.ChangeType}");
         AddToCache(e.FullPath);
         //FilesToProcessDictionary.TryAdd(e.FullPath, e.FullPath); // // Use this for FilesToProcess ConcurrentDictionary
      }
      private static void FileCreated(object sender, FileSystemEventArgs e)
      {
         WriteLine($"* File created: {e.Name} - type: {e.ChangeType}");

         //var fileProcessor = new FileProcessor(e.FullPath);
         //fileProcessor.Process();
         
         AddToCache(e.FullPath);

         //FilesToProcessDictionary.TryAdd(e.FullPath, e.FullPath); // Use this for FilesToProcess ConcurrentDictionary
      }
      #endregion
      private static void AddToCache(string fullPath)
      {
         var item = new CacheItem(fullPath, fullPath);

         var policy = new CacheItemPolicy
         {
            RemovedCallback = ProcessFilesWithCache,
            SlidingExpiration = TimeSpan.FromSeconds(2),
         };

         FilesToProcessCache.Add(item, policy);

      }
      private static void ProcessFilesWithCache(CacheEntryRemovedArguments args)
      {
         WriteLine($"* Cache item removed: {args.CacheItem.Key} because {args.RemovedReason}");

         if(args.RemovedReason == CacheEntryRemovedReason.Expired)
         {
            var fileProcessor = new FileProcessor(args.CacheItem.Key);
            fileProcessor.Process();
         }
         else
         {
            WriteLine($"WARNING: {args.CacheItem.Key} was removed unexpectedly and may not be processed");
         }
      }
      private static void ProcessFilesWithConcurrentDictionary(Object stateInfo)
      {
         foreach(var fileName in FilesToProcessDictionary.Keys)
         {
            if(FilesToProcessDictionary.TryRemove(fileName, out _))
            {
               var fileProcessor = new FileProcessor(fileName);
               fileProcessor.Process();
            }
         }
      }

   }
}
