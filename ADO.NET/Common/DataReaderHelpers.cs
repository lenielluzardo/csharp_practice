using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace ADO.NET.Common
{
   public static class DataReaderHelpers
   {
      public static T GetFieldValue<T>(this SqlDataReader dr, string name)
      {
         T ret = default;
         if (!dr[name].Equals(DBNull.Value))
         {
            ret = (T)dr[name];
         }

         return ret;
      }
   }
}
