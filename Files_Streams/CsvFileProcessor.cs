using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Files_Streams.Models;

namespace Files_Streams
{
   public class CsvFileProcessor
   {
      private readonly IFileSystem _fileSystem;
      public CsvFileProcessor(string inputFilePath, string outputFilePath) : this(inputFilePath, outputFilePath, new FileSystem()) { }
      public CsvFileProcessor(string inputFilePath, string outputFilePath, IFileSystem fileSystem)
      {
         InputFilePath = inputFilePath;
         OutputFilePath = outputFilePath;
         _fileSystem = fileSystem;

         CsvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
         {
            Comment = '@',
            AllowComments = true,
            TrimOptions = TrimOptions.Trim,
            //Delimiter = ";",
            //HasHeaderRecord = false,
            //IgnoreBlankLines = false,

            // For entities that doesn't match the structure of .csv **WITHOUT MAPPING CONFIGURATION**
            //HeaderValidated = null, 
            //MissingFieldFound = null,

            // For entities that doesn't match the structure of .csv **WITH MAPPING CONFIGURATION**
          

         };

      }

      public string InputFilePath { get; }
      public string OutputFilePath { get; }
      public CsvConfiguration CsvConfiguration { get; set; }

      public void Process()
      {
         ProcessAsEntityCsv();
      }

      private void ProcessAsEntityCsv()
      {
         using (StreamReader input = _fileSystem.File.OpenText(InputFilePath))
         using (CsvReader csvReader = new CsvReader(input, CsvConfiguration))
         using (StreamWriter output = _fileSystem.File.CreateText(OutputFilePath))
         using (var csvWriter = new CsvWriter(output, CsvConfiguration))
         {
            IEnumerable<ProcessedOrder> records = csvReader.GetRecords<ProcessedOrder>();

            csvReader.Context.RegisterClassMap<ProcessedOrderMap>();

            //csvWriter.WriteRecords(records); // Writes all record leaving at the end of the csv a blank line

            csvWriter.WriteHeader<ProcessedOrder>();
            csvWriter.NextRecord();

            var recordsArray = records.ToArray();
            for(int i = 0; i < recordsArray.Length; i++)
            {
               csvWriter.WriteField(recordsArray[i].OrderNumber);
               csvWriter.WriteField(recordsArray[i].Customer);
               csvWriter.WriteField(recordsArray[i].Amount);

               bool isLastRecord = i == recordsArray.Length - 1;

               if (!isLastRecord)
               {
                  csvWriter.NextRecord();
               }

            }
            
         }
      }

      private void ProcessAsPlainCsv()
      {
         using(StreamReader input = File.OpenText(InputFilePath))
         using(CsvReader csvReader = new CsvReader(input, CsvConfiguration))
         {
            IEnumerable<dynamic> records = csvReader.GetRecords<dynamic>();

            foreach (var record in records)
            {
               Console.WriteLine("{0}", record.OrderNumber);
               Console.WriteLine("{0}", record.CustomerNumber);
               Console.WriteLine("{0}", record.Description);
               Console.WriteLine("{0}", record.Quantity);
            }
         }
      }
   }
}
