using Xunit;
using System.IO.Abstractions.TestingHelpers;
using Output.Files_Streams;
using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;

namespace xUnitTests.Files_Streams
{
   public class FileProcessorShould
   {
      private const string _inputPath = @"c:\root\in\";
      private const string _outputPath = @"c:\root\out\";

      private MockFileSystem SetUpFileSystem(string file, MockFileData mockInputFile)
      {
         var mockFileSystem = new MockFileSystem();
         mockFileSystem.AddFile(@$"c:\root\in\{file}", mockInputFile);
         mockFileSystem.AddDirectory(@$"c:\root\out");

         return mockFileSystem;
      }

      [Fact]
      public void ProcessTextFile_And_MakeSecondLineUpperCase()
      {
         var fileName = "myfile.txt";

         //Create a mock input file
         var mockInputFile = new MockFileData("Line 1\nLine 2\nLine 3");

         //Setup mock file system starting state
         var mockFileSystem = SetUpFileSystem("myfile.txt", mockInputFile);

         //Create TextFileProcessor with mock file system
         var sut = new TextFileProcessor(_inputPath + fileName, _outputPath + fileName, mockFileSystem);

         //Process test file
         sut.Process();

         //Check mock file system for output file
         Assert.True(mockFileSystem.FileExists(@"c:\root\out\myfile.txt"));

         //Check content of output file in mock file system
         MockFileData processedFile = mockFileSystem.GetFile(@"c:\root\out\myfile.txt");

         string[] lines = processedFile.TextContents.SplitLines();

         Assert.Equal("Line 1", lines[0]);
         Assert.Equal("LINE 2", lines[1]);
         Assert.Equal("Line 3", lines[2]);

      }

      [Fact]
      public void ProcessBinaryFile_And_AddLargestNumber()
      {
         var fileName = "myfile.data";

         var mockInputFile = new MockFileData(new byte[] { 0xFF, 0x34, 0x56, 0xD2 });

         var mockFileSystem = SetUpFileSystem(fileName, mockInputFile);

         var sut = new BinaryFileProcessor(_inputPath + fileName, _outputPath + fileName, mockFileSystem);

         sut.Process();

         Assert.True(mockFileSystem.FileExists(_outputPath + fileName));

         var processedFile = mockFileSystem.GetFile(_outputPath + fileName);

         var data = processedFile.Contents;

         Assert.Equal(5, data.Length);
         Assert.Equal(0xFF, data[4]);

      }

      [Fact]
      [UseReporter(typeof(DiffReporter))]
      public void ProcessCsvFile_And_OutputProcessedCsvData()
      {
         var fileName = "myfile.csv";

         var csvLines = new StringBuilder();
         csvLines.AppendLine("OrderNumber,CustomerNumber,Description,Quantity");
         csvLines.AppendLine("42,100001,Shirt,II");
         csvLines.AppendLine("43,200002,Shorts,I");
         csvLines.AppendLine("@ This is a comment");
         csvLines.Append("44,300003,Cap,V");

         var mockInputFile = new MockFileData(csvLines.ToString());

         var mockFileSystem = SetUpFileSystem(fileName, mockInputFile);

         var sut = new CsvFileProcessor(_inputPath + fileName, _outputPath + fileName, mockFileSystem);

         sut.Process();

         Assert.True(mockFileSystem.FileExists(_outputPath + fileName));

         var processedFile = mockFileSystem.GetFile(_outputPath + fileName);

         //var lines = processedFile.TextContents.SplitLines();

         Approvals.Verify(processedFile.TextContents);

         //Assert.Equal("OrderNumber,Customer,Amount", lines[0]);
         //Assert.Equal("42,100001,2", lines[1]);
         //Assert.Equal("43,200002,1", lines[2]);
         //Assert.Equal("44,300003,5", lines[3]);

      }
   }
}
