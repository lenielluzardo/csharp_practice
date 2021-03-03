using CsvHelper.Configuration;
using ZZ_Common.Models;
using System.Globalization;

namespace Files_Streams.Helpers
{
   public class ProcessedOrderMap : ClassMap<ProcessedOrder>
   {
      public ProcessedOrderMap()
      {
         AutoMap(CultureInfo.InvariantCulture);

         Map(m => m.Customer).Name("CustomerNumber");
         Map(m => m.Amount).Name("Quantity").TypeConverter<RomanTypeConverter>();
      }
   }
}
