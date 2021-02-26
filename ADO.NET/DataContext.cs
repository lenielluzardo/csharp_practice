using ADO.NET.Models;
using ADO.NET.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

/// <summary>
/// Examples of how to implement the ADO.NET Class Library, to connect to databases and work with data.
/// </summary>
namespace ADO.NET
{
   public class DataContext
   {
      public string ConnectionString { get; set; }
      public int RowsAffected { get; set; }
      public string ResultText { get; set; }

      public List<Item> Items = new List<Item>();
      public List<Category> Categories = new List<Category>();


      #region Raw Reading Operations
      public void ConnectManually()
      {
         // Create a new connection
         SqlConnection cnn = new SqlConnection(ConnectionString);

         //Opens the connection
         cnn.Open();

         // Closes the connection
         cnn.Close();

         // Dispose all unmanaged resources
         cnn.Dispose();
      }
      public void ConnectDynamically()
      {
         try
         {
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
               cnn.Open();
            }
         }
         catch (Exception ex)
         {

         }
      }
      public int GetItemsCountScalar()
      {
         string sql = "SELECT COUNT(*) FROM Item";

         using (SqlConnection cnn = new SqlConnection(ConnectionString))
         {
            using (SqlCommand cmd = new SqlCommand(sql, cnn))
            {
               cnn.Open();
               //Retunrs an object data type. Cast it to the data type you know it returns based on the SQL submitted.
               return RowsAffected = (int)cmd.ExecuteScalar();
            }
         }
      }
      public int InsertItem()
      {
         RowsAffected = 0;

         //Create SQL statement to submit.
         string sql = @"INSER INTO Item(ItemName, ItemDate, Price)
                        VALUES('My first inserted item', '2020-02-26', 10.00)";

         try
         {
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
               using (SqlCommand cmd = new SqlCommand(sql, cnn))
               {
                  cmd.CommandType = CommandType.Text;
                  cnn.Open();

                  //Execute the INSERT statement.
                  RowsAffected = cmd.ExecuteNonQuery();

               }
            }
         }
         catch (Exception ex)
         {

         }

         return RowsAffected;
      }
      public int GetItemsUsingScalarParameters(string productName)
      {
         string sql = @"SELECT COUNT(*) FROM Item
                        WHERE ItemName LIKE @ItemName";

         using (SqlConnection cnn = new SqlConnection(ConnectionString))
         {
            using (SqlCommand cmd = new SqlCommand(sql, cnn))
            {
               cmd.Parameters.Add(new SqlParameter("@ItemName", productName));

               cnn.Open();

               return RowsAffected = (int)cmd.ExecuteScalar();
            }
         }
      }
      public void InsertItemOutputParameters(string itemName, DateTime itemDate, double itemPrice)
      {
         RowsAffected = 0;

         //Name of Stored Procedure.
         string sql = "Item_Insert";

         try
         {
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
               using (SqlCommand cmd = new SqlCommand(sql, cnn))
               {
                  cmd.Parameters.Add(new SqlParameter("@ItemName", itemName));
                  cmd.Parameters.Add(new SqlParameter("@ItemDate", itemDate));
                  cmd.Parameters.Add(new SqlParameter("@ItemPrice", itemPrice));

                  //Create output parameter.
                  cmd.Parameters.Add(new SqlParameter { ParameterName = "@ItemId", Value = 1, IsNullable = false, DbType = DbType.Int32, Direction = ParameterDirection.Output });

                  cmd.CommandType = CommandType.StoredProcedure;

                  cnn.Open();

                  RowsAffected = cmd.ExecuteNonQuery();

                  //Get output paramater.
                  //Any Output parameter is automatically filled in to the appropriate parameter by ADO.NET.1
                  var itemId = (int)cmd.Parameters["@ItemItem"].Value;
               }
            }
         }
         catch (Exception ex) { }
      }
      public int TransactionProcessing()
      {
         RowsAffected = 0;

         string sql = @"INSER INTO Item(ItemName, ItemDate, ItemPrice)
                        VALUES(@ItemName, @ItemDate, @ItemPrice)";

         //Use at least one try..catch block when performing transaction processing.
         try
         {
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
               using (SqlTransaction trn = cnn.BeginTransaction())
               {
                  try
                  {
                     using (SqlCommand cmd = new SqlCommand(sql, cnn))
                     {
                        //Assign transaction object to the command.
                        cmd.Transaction = trn;

                        //Create Parameters.
                        //Set the command type.

                        cnn.Open();
                        RowsAffected = cmd.ExecuteNonQuery();

                        //***** SECOND STATEMENT TO EXECUTE *****
                        sql = "NEW SQL STATEMENT TO PERFORM";

                        //Asign the new SQL statement to the command object.
                        cmd.CommandText = sql;

                        //Clear old parameters.
                        cmd.Parameters.Clear();

                        //Create new parameters for the new SQL statement.
                        cmd.Parameters.Add(new SqlParameter("@ItemName", "Value of parameter"));

                        RowsAffected = cmd.ExecuteNonQuery();

                        //Finish the transaction. Both command will be commited to the DB.
                        trn.Commit();
                     }
                  }
                  catch (Exception ex) { trn.Rollback(); }
               }
            }
         }
         catch (Exception ex) { /* Do Something */}
         return RowsAffected;
      }
      #endregion

      #region Data Reader Operations
      public void GetItemsWithDataReader()
      {
         StringBuilder sb = new StringBuilder(1024);

         using (SqlConnection cnn = new SqlConnection(ConnectionString))
         {
            using (SqlCommand cmd = new SqlCommand("", cnn))
            {
               cnn.Open();

               //The command behaviour enumeration should always be used with the ExecuteReader() method.
               //It means that when the reader is closed the connections is closed too.
               using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
               {
                  while (dr.Read())
                  {
                     //Here we use string builder but we can map to an entity.
                     sb.AppendLine("Item: " + dr["ItemId"].ToString());
                     sb.AppendLine(dr["ItemName"].ToString());
                     sb.AppendLine(Convert.ToDateTime(dr["ItemDate"]).ToShortDateString());
                     sb.AppendLine(Convert.ToDecimal(dr["ItemPrice"]).ToString("c"));
                     sb.AppendLine();

                     //Data reader provides a set of methods which allows us to get specific value types from DB.
                     /* 
                     dr.GetString();
                     dr.GetGuid();
                     dr.GetInt32();  
                     dr.GetOrdinal() || Returns the value based on the given column name.
                     */
                  }
               }
            }
         }
      }
      public void GetItemsWithDataReaderExtensionMethod()
      {
         using (SqlConnection cnn = new SqlConnection(ConnectionString))
         {
            using (SqlCommand cmd = new SqlCommand("", cnn))
            {
               cnn.Open();

               using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
               {
                  while (dr.Read())
                  {
                     Items.Add(new Item
                     {
                        //Using extension helper to simplify the mapping.
                        //Internally it checks if value is null. 
                        //In case we know that some column in the DB is nullable we need to check it manually.

                        Id = dr.GetFieldValue<int>("ItemId"),
                        Name = dr.GetFieldValue<string>("ItemName"),
                        Date = dr.GetFieldValue<DateTime>("ItemDate"),
                        Price = dr.GetFieldValue<double>("ItemPrice"),

                     });
                  }
               }
            }
         }
      }
      public void GetMultipleResultSets()
      {
         Items.Clear();
         Categories.Clear();

         string sql = "First SQL Statement; Second SQL Statement";

         using (SqlConnection cnn = new SqlConnection(ConnectionString))
         {
            using (SqlCommand cmd = new SqlCommand(sql, cnn))
            {
               cnn.Open();

               using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
               {
                  while (dr.Read())
                  {
                     Items.Add(new Item
                     {
                        //Do the mapping.
                     });
                  }

                  //Move to the next result of the SQL Statement.
                  dr.NextResult();
                 
                  while (dr.Read())
                  {
                     Categories.Add(new Category
                     {
                        //Do the mapping.
                     });
                  }
               }
            }
         }
      }
      #endregion

      #region Working with Exceptions
      public void GatherExceptionInformation()
      {
         SqlConnection cnn = null;
         SqlCommand cmd = null;
         try
         {
            string sql = "Store_Procedure";

            cnn = new SqlConnection(ConnectionString);
            cmd = new SqlCommand(sql, cnn);

            cmd.Parameters.Add(new SqlParameter("@Name", "Generate Exception"));
            cmd.CommandType = CommandType.StoredProcedure;

            cnn.Open();

            RowsAffected = cmd.ExecuteNonQuery();
         }
         catch(SqlException ex)
         {
            //Is an Singleton Instance
            SqlServerDataException.Instance.Publish(ex, cmd, "Error produced in: [class name]");
            ResultText = SqlServerDataException.Instance.LastException.ToString();
         }
         catch(Exception ex)
         {
            ResultText = ex.Message;
         }
      }
      #endregion

      #region Data Tables
      public DataTable GetItemsAsDataTable()
      {
         DataTable dt = null;

         using(SqlConnection cnn = new SqlConnection(ConnectionString))
         {
            using(SqlCommand cmd = new SqlCommand("", cnn))
            {
               using(SqlDataAdapter da = new SqlDataAdapter(cmd))
               {
                  dt = new DataTable();

                  //Fill DataTable using DataAdapter.
                  //Fill() method uses a DataReader to populate the DataTable.
                  da.Fill(dt);

                  ProcessRowsAndColumns(dt);
                  return dt;
               }
            }
         }
      }
      public void ProcessRowsAndColumns(DataTable dt)
      {
         StringBuilder sb = new StringBuilder(2048);
         int index = 1;
         
         // Process each row
         foreach(DataRow row in dt.Rows)
         {
            sb.AppendLine("**Row: " + index.ToString() + "   **");

            //Process each column
            foreach(DataColumn col in dt.Columns)
            {
               //This gives the column name and the value inside the row/colum name
               sb.AppendLine(col.ColumnName + ": " + row[col.ColumnName].ToString());
            }
            
            sb.AppendLine();
            index++;
         }
      }
      public List<Item> GetItemsAsListUsingDataTable()
      {
         List<Item> ret = new List<Item>();
         ResultText = string.Empty;
         RowsAffected = 0;

         DataTable dt = null;

         string sql = "Some SQL Statement";

         using(SqlConnection cnn = new SqlConnection(ConnectionString))
         {
            using(SqlCommand cmd = new SqlCommand(sql, cnn))
            {
               cmd.Parameters.Add(new SqlParameter("@ItemName", "Name of Item"));
               using(SqlDataAdapter da = new SqlDataAdapter(cmd))
               {
                  dt = new DataTable();
                  da.Fill(dt);
                  if(dt.Rows.Count > 0)
                  {
                     ret = (from row in dt.AsEnumerable() //Must convert to an enumerable object
                            select new Item()
                            {
                               //Mapping
                               Name = row.Field<string>("ItemName"),
                               Date = row.Field<DateTime>("ItemDate"),
                               Price = row.Field<double>("ItemPrice")
                            }).ToList();
                  }  
               }
            }
         }
         return ret;
      }
      public void GetItemsAsDataSetMultipleResultSets()
      {
         ResultText = string.Empty;
         RowsAffected = 0;
         DataSet ds = new DataSet();

         string sql = "Some SQL Statement";

         using(SqlConnection cnn = new SqlConnection(ConnectionString))
         {
            using(SqlCommand cmd = new SqlCommand(sql, cnn))
            {
               using(SqlDataAdapter da = new SqlDataAdapter(cmd))
               {
                  //The SqlDataAdapter class identifies the type of object passed to the Fill() method. 
                  //If it is a DataSet and we have multiple results, it creates individual DataTable objects for each.
                  da.Fill(ds);

                  if(ds.Tables.Count > 0)
                  {
                     var items = ds.Tables[0];
                     var categories = ds.Tables[1];
                  }
               }
            }
         }
      }
      public DataView GetItemsAsDataView(double lessThan)
      {
         DataView dv = null;
         DataTable dt = GetItemsAsDataTable();
         if(dt != null)
         {
            dv = dt.DefaultView;

            //The Sort property takes any column name to sort by.
            //We can include ASC or DESC to sort ascending or descending.
            //Ascending is the default.
            dv.Sort = "Price DESC";

            //Is like a SQL 'WHERE' statement without the 'WHERE' clause when setting the RowFilter property.
            //Can filter using And, Or, True, False, Is, Like, etc.
            dv.RowFilter = "Price < " + lessThan.ToString();

            //** OR THE SAME AS ABOUVE BUT USING LINQ **
            dv = (from item in dt.AsEnumerable()
                  where item.Field<double>("Price") < lessThan
                  orderby item.Field<double>("Price")
                  select item).AsDataView();
            
            RowsAffected = dv.Count;

            var dtFiltered = dv.ToTable();
         }
         return dv;
      }
      public DataTable CreateDataTable()
      {
         DataTable dt = new DataTable();
         DataColumn dc;

         //*** Creating Columns ***
         //Method 1: Use Add() method
         dt.Columns.Add("ItemId", typeof(int));

         //Method 2: Create a DataColumn object for mor control.
         dc = new DataColumn
         {
            DataType = typeof(string),
            ColumnName = "ProductName",
            Caption = "Product Name",
            ReadOnly = true
         };

         //Method 3: Create a DataColumn object in Add() method, combines the two options above.
         dt.Columns.Add(new DataColumn
         {
            DataType = typeof(double),
            ColumnName = "Price",
            Caption = "Price",
            ReadOnly = false
         });

         //*** Adding Rows ***
         //Method 1: Pass in variable amount of arguments to Add() method.
         dt.Rows.Add(1, "Working with ADO.NET", 1.0);

         //Method 2: Create a new DataRow object.
         DataRow dr = dt.NewRow();
         //Set each column by name.
         dr["ItemId"] = 2;
         dr["ItemName"] = "Working with ADO.NET";
         dr["ItemPrice"] = 20.0;

         dt.Rows.Add(dr);

         dt.AcceptChanges();

         return dt;
      }
      public DataTable CloneDataTable()
      {
         DataTable dt = CreateDataTable();
         
         //Copy the structure of a DataTable.
         DataTable dtNew = dt.Clone();

         return dtNew;
      }
      public DataTable CopyDataTable()
      {
         DataTable dt = CreateDataTable();

         //Copy the structure and the data of a DataTable.
         DataTable dtNew = dt.Copy();

         return dtNew;
      }
      public DataTable SelectCopyRows()
      {
         DataTable dt;
         DataTable dtNew;

         dt = GetItemsAsDataTable();

         dtNew = dt.Clone();

         //Select Method is like a 'WHERE' clause.
         DataRow[] rows = dt.Select("Price < 20");

         foreach (DataRow row in rows)
         {
            //NOTE: The following causes an error.
            //      A single row cannot belong to more than one data table.
            //dtNew.Rows.Add(row);

            //Method 1: Use ItemArray to avoid the error.
            //dtNew.Rows.Add(row.ItemArray);

            //Method 2: Use ImportRow to avoid the error.
            dtNew.ImportRow(row);
         }
         dtNew.AcceptChanges();

         //** THE SAME OF ABOVE BUT WITHOUT LOOPING **
         dtNew = dt.Select("Price < 20").CopyToDataTable();

         return dtNew;
      }
      #endregion

      #region Builder Classes
      public string BreakApartConnectionString()
      {
         StringBuilder sb = new StringBuilder(1024);

         SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);

         //Access each property of the connection string.
         sb.AppendLine("ApplicationName: " + builder.ApplicationName);
         sb.AppendLine("DataSource: " + builder.DataSource);
         sb.AppendLine("InitialCatalog: " + builder.InitialCatalog);
         sb.AppendLine("UserID: " + builder.UserID);
         sb.AppendLine("Password: " + builder.Password);
         sb.AppendLine("IntegratedSecurity: " + builder.IntegratedSecurity.ToString());

         return sb.ToString();

         //**OR CREATE A NEW CONNECTIONSTRING**
         //Creates the connection string in the right order.
         //var cnnString = builder.ToString();
      }
      public string CreateSqlCommand()
      {
         ResultText = string.Empty;
         try
         {
            using(SqlConnection cnn = new SqlConnection(ConnectionString))
            {
               using(SqlDataAdapter da = new SqlDataAdapter("sql command", cnn))
               {
                  DataTable dt = new DataTable();
                  da.Fill(dt);

                  using(SqlCommandBuilder builder = new SqlCommandBuilder(da))
                  {
                     //Build INSERT command.
                     ResultText = "Insert: " + builder.GetInsertCommand(true).CommandText;
                     ResultText += Environment.NewLine;

                     //Build UPDATE command.
                     ResultText += "Update: " + builder.GetUpdateCommand(true).CommandText;
                     ResultText += Environment.NewLine;

                     //Build DELETE command. 
                     ResultText += "Delete: " + builder.GetDeleteCommand(true).CommandText;

                     //Using the SQL Command Builder In a SQL Command.
                     using (SqlCommand cmd = builder.GetInsertCommand(true))
                     {
                        cmd.Parameters["@ItemName"].Value = "A Input Value From Somewhere Else";
                        cmd.Parameters["@ItemDate"].Value = DateTime.Now;
                        cmd.Parameters["@ItemPrice"].Value = 20.0;

                        cmd.Connection = cnn;

                        var result = cmd.ExecuteNonQuery();
                     }

                  }
               }
            }
         }catch(Exception ex) { }
         return ResultText;

      }

      #endregion
   }
}