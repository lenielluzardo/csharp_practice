using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Http_Client
{
   /// <summary>
   /// Reads and Deserializes from Stream of the given Type.
   /// </summary>
   public static class StreamExtensions
   {
      public static T ReadAndDeserializeFromJson<T>(this Stream stream)
      {
         if(stream == null)
         {
            throw new ArgumentNullException();
         }
         if (!stream.CanRead)
         {
            throw new NotSupportedException("Can't read from this stream");
         }

         using (StreamReader streamReader = new StreamReader(stream))
         {
            using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
            {
               var jsonSerializer = new JsonSerializer();
               return jsonSerializer.Deserialize<T>(jsonTextReader);
               //do something with poster.
            }
         }
      }
      public static void SerializeJsonAndWrite<T>(this Stream stream, T objectToWrite)
      {
         if(stream == null)
         {
            throw new ArgumentNullException(nameof(stream));
         }
         if (!stream.CanWrite)
         {
            throw new NotSupportedException("Can't write to this stream");
         }

         using(var streamWriter = new StreamWriter(stream, new UTF8Encoding(), 1024, true))
         {
            using(var jsonWriter = new JsonTextWriter(streamWriter))
            {
               var jsonSerializer = new JsonSerializer();
               jsonSerializer.Serialize(jsonWriter, objectToWrite);
               jsonWriter.Flush();
            }
         }
      }
   }
}
