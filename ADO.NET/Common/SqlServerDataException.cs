using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;

namespace ADO.NET.Common
{
   public class SqlServerDataException: Exception
   {
      protected SqlServerDataException(string msg, Exception ex) 
      {
         //Should implement singleton pattern
      }
      public static SqlServerDataException Instance { get; set; }
      public SqlParameterCollection CommandParameters { get; set; }
      public string ConnectionString { get; set; }
      public string Database { get; set; }
      public string SQL { get; set; }
      public string WorkstationId { get; set; }
      public Exception LastException { get; set; }

      public void Publish(Exception ex, SqlCommand cmd, string exceptionMsg)
      {
         LastException = ex;

         if(cmd != null)
         {
            LastException = CreateDbException(ex, cmd, null);
            Debug.WriteLine(ex.ToString());
         }
      }
      public virtual SqlServerDataException CreateDbException(Exception ex, SqlCommand cmd, string exceptionMsg)
      {
         SqlServerDataException exc;
         exceptionMsg = string.IsNullOrEmpty(exceptionMsg) ? string.Empty : exceptionMsg;

         exc = new SqlServerDataException(exceptionMsg + ex.Message, ex)
         {
            ConnectionString = cmd.Connection.ConnectionString,
            Database = cmd.Connection.Database,
            SQL = cmd.CommandText,
            CommandParameters = cmd.Parameters,
            WorkstationId = Environment.MachineName,
         };

         return exc;
      }

      protected virtual string GetCommandParametersAsString() 
      {
         StringBuilder ret = new StringBuilder(1024);

         if(CommandParameters != null)
         {
            if(CommandParameters.Count > 0)
            {
               ret = new StringBuilder(1024);

               foreach(IDbDataParameter param in CommandParameters)
               {
                  ret.Append(" " + param.ParameterName);
                  if (param.Value == null)
                     ret.AppendLine(" = null");
                  else
                     ret.AppendLine(" = " + param.Value.ToString());
               }
            }
         }
         return ret.ToString();
      }
      public void GetDatabaseSpecificError() { }
      public void GetInnerExceptionInfo() { }
      public void HideLoginInfoForConnectionString() { }
      public void IsDatabaseSpecificError() { }
      public override string ToString() 
      {
         StringBuilder sb = new StringBuilder(1024);

         sb.AppendLine(new string('-', 80));
         if (!string.IsNullOrEmpty(Message))
         {
            sb.AppendLine("Type: " + this.GetType().FullName);
         }
         if (!string.IsNullOrEmpty(Message))
         {
            sb.AppendLine("Message: " + Message);
         }
         if (!string.IsNullOrEmpty(Database))
         {
            sb.AppendLine("Database: "+ Database);
         }

         //Check command parameters you want
         sb.AppendLine("Parameters: ");
         sb.Append(GetCommandParametersAsString());

         //Check more elements

         return sb.ToString();
      }

   }
}
