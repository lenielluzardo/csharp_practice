using System.IO;

namespace Output.Files_Streams
{
   public class TextFileProcessor
   {
      public TextFileProcessor(string inputFilePath, string outputFilePath)
      {
         InputFilePath = inputFilePath;
         OutputFilePath = outputFilePath;

      }

      public string InputFilePath { get; }
      public string OutputFilePath { get; }

      public void Process()
      {
         ProcessAsStream();
      }
      
      private void ProcessAsABlock()
      {
         string originalText = File.ReadAllText(InputFilePath);
         string processedText = originalText.ToUpperInvariant();
         File.WriteAllText(OutputFilePath, processedText);
      }

      private void ProcessAsSeparatedLines()
      {
         string[] lines = File.ReadAllLines(InputFilePath); 
         lines[1] = lines[1].ToUpperInvariant(); // Assumes there is a line 2 in the file
         File.WriteAllLines(OutputFilePath, lines); // Appends a line at the end of the file
      }

      private void ProcessAsStream()
      {
         //using (var inputFileStream = new FileStream(InputFilePath, FileMode.Open)) 
         //using(var inputStreamReader = new StreamReader(inputFileStream))
         //using (var outputFileStream = new FileStream(OutputFilePath, FileMode.Create))
         //using (var outputStreamWriter = new StreamWriter(outputFileStream))

         //Simplify the creation of the stream reader and writer
         using (var inputStreamReader = new StreamReader(InputFilePath))
         using (var outputStreamWriter = new StreamWriter(OutputFilePath))
         {
            var currentLineNumber = 1;
            
            while (!inputStreamReader.EndOfStream)
            {
               string line = inputStreamReader.ReadLine();

               if(currentLineNumber == 2)
               {
                  Write(line.ToUpperInvariant());
               }
               else
               {
                  Write(line);
               }

               currentLineNumber++;
               
               void Write(string content)
               {
                  bool isLastLine = inputStreamReader.EndOfStream;
                  if (isLastLine)
                  {
                     outputStreamWriter.Write(content);
                  }
                  else
                  {
                     outputStreamWriter.WriteLine(content);
                  }
               }
            }
         }
      }
   }
}
