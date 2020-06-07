using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaymsWPFBoiler.Data.Contexts;
using DaymsWPFBoiler.Data.Resources;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Storage;

namespace DaymsWPFBoiler.Data.Utilities
{
    public static class DBInitializer
    {
        // Initializing the logger in this class.
        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static readonly Dictionary<string, string> UniqueColumns = new Dictionary<string, string>
        {
            // If Needed, add here.
        };

        private static readonly Dictionary<string, string> ColumnNames = new Dictionary<string, string>();

        private static readonly Dictionary<string, Dictionary<long, Guid>> TableIds = new Dictionary<string, Dictionary<long, Guid>>();
        
        private static readonly string ByteType = typeof(byte[]).Name;

        #region Dayms Database Setup

        public static void InitializeDB()
        {
            // Initialize Database
            using (DaymsContext context = new DaymsContext())
            {
                IDbContextTransaction transaction = context.Database.BeginTransaction();
                transaction.Dispose();

                // Grab Database Connection
                SqliteConnection connection = context.GetConnection();

                /* Create necessary database objects */
                CreateEssentialDatabaseObjects(connection);

                // Insert Default Values for tables like Users, Roles, etc.
                InsertDefaultValues(connection);
            }
        }

        private static void CreateEssentialDatabaseObjects(SqliteConnection connection)
        {

        }

        private static void InsertDefaultValues(SqliteConnection connection)
        {

            #region Users

            Insert(
                connection: connection,
                ignoreOnConflict: true,
                tableName: ConnectionStrings.DB_TableName_Users,
                columnNames: "Username,Password,FirstName,LastName,EmailAddress,IsAdmin",
                values: "'admin','2CSU8F1pF7oC96qilonMtES7c/IDgIdssF0fN1N7eJI=','Initial','Admin','email@address.com','true'"
            );

            #endregion

            #region Roles

            Insert(
                connection: connection, 
                ignoreOnConflict: true, 
                tableName: ConnectionStrings.DB_TableName_Roles, 
                columnNames: "Name",
                    "'Super Admin'",
                    "'Admin'",
                    "'User'"
            );

            #endregion
        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// Helper function to drop table(s)
        /// </summary>
        /// <param name="connection">SqliteConnection</param>
        /// <param name="tableNames">Name(s) of table(s) in DB</param>
        /// <returns></returns>
        private static bool DropTables(SqliteConnection connection, params string[] tableNames)
        {
            foreach (string tableName in tableNames)
            {
                try
                {
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "DROP TABLE " + tableName + ConnectionStrings.DB_Semicolon;

                        int affectedRows = command.ExecuteNonQuery();

                        if (affectedRows < 0)
                        {
                            return false;
                        }
                    }
                }
                catch (Exception tableException)
                {
                    Logger.Error(tableException, "DBI104");
                    throw;
                }
            }
            return true;
        }

        /// <summary>
        /// Helper function to create DB table
        /// </summary>
        /// <param name="connection">SqliteConnection</param>
        /// <param name="tableName">Name of table in DB</param>
        /// <param name="columns">Any additional columns with column definitions to initialize the table with</param>
        private static bool CreateBareTable(SqliteConnection connection, string tableName, params string[] columns)
        {
            try
            {
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = ConnectionStrings.DB_CreateTable + ConnectionStrings.DB_IfNotExists +
                        tableName + ConnectionStrings.DB_OpenParenthesis + "Id " + ConnectionStrings.DB_Constraints_Guid;

                    if (columns.Length > 0 && columns.ToList() is List<string> cols)
                    {
                        cols.ForEach(s => command.CommandText += ConnectionStrings.DB_Comma + s);
                    }

                    command.CommandText += ConnectionStrings.DB_CloseParenthesis +
                        ConnectionStrings.DB_Semicolon;

                    int affectedRows = command.ExecuteNonQuery();

                    if (affectedRows >= 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (System.Exception tableException)
            {
                Logger.Error(tableException, "DBI009");
                throw;
            }

        }

        /// <summary>
        /// Helper function to create DB table
        /// </summary>
        /// <param name="connection">SqliteConnection</param>
        /// <param name="tableName">Name of table in DB</param>
        /// <param name="columns">Any additional columns with column definitions to initialize the table with</param>
        private static bool CreateTable(SqliteConnection connection, string tableName, params string[] columns)
        {
            string dateCreatedColumn = "DateCreated " + ConnectionStrings.DB_Constraints_DateTime;
            string dateModifiedColumn = "DateModified " + ConnectionStrings.DB_Constraints_DateTime;
            columns = new string[] { dateCreatedColumn, dateModifiedColumn }.ToList().Concat(columns.ToList()).ToArray();
            return CreateBareTable(connection, tableName, columns);
        }

        /// <summary>
        /// Helper function to create a lookup table in DB
        /// </summary>
        /// <param name="connection">SqliteConnection</param>
        /// <param name="tableName">Name of table in DB</param>
        private static void CreateLookupTable(SqliteConnection connection, string tableName)
        {
            try
            {
                string nameColumn = UniqueColumns[tableName] + " " + ConnectionStrings.DB_ColType_Varchar + " " +
                        ConnectionStrings.DB_Constraint_Unique + " " + ConnectionStrings.DB_Constraint_NotNull;

                // Create Table, if needed
                if (CreateTable(connection, tableName, nameColumn))
                {
                    // If table was just created, create trigger to set DateModified after update
                    CreateDateModifiedTrigger(connection, tableName);
                }
            }
            catch (System.Exception lookupException)
            {
                Logger.Error(lookupException, "DBI008");
            }
        }

        /// <summary>
        /// Helper function to get type of Id column in table
        /// </summary>
        /// <param name="connection">SqliteConnection</param>
        /// <param name="tableName">Name of table in DB</param>
        /// <returns></returns>
        private static bool HasGuidIdType(SqliteConnection connection, string tableName)
        {
            try
            {
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = ConnectionStrings.DB_Pragma_TableInfo + ConnectionStrings.DB_OpenParenthesis +
                        tableName + ConnectionStrings.DB_CloseParenthesis + ConnectionStrings.DB_Semicolon;
                    SqliteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        if (reader.GetString(1) == "Id" && reader.GetString(2) == "BLOB")
                        {
                            return true;
                        }
                    }
                }
            }
            catch (System.Exception guidIdTypeException)
            {
                Logger.Error(guidIdTypeException, "DBI110");
            }

            return false;
        }

        /// <summary>
        /// Helper function to get names of all columns in table
        /// </summary>
        /// <param name="connection">SqliteConnection</param>
        /// <param name="tableName">Name of table in DB</param>
        /// <returns></returns>
        private static List<string> GetColumnNames(SqliteConnection connection, string tableName)
        {
            List<string> columnNames = new List<string>();

            try
            {
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = ConnectionStrings.DB_Pragma_TableInfo + ConnectionStrings.DB_OpenParenthesis +
                        tableName + ConnectionStrings.DB_CloseParenthesis + ConnectionStrings.DB_Semicolon;
                    SqliteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        columnNames.Add(reader.GetString(1));
                    }
                }
            }
            catch (System.Exception columnNameException)
            {
                Logger.Error(columnNameException, "DBI010");
            }

            return columnNames;
        }

        /// <summary>
        /// Helper function to get names of all columns in table as a comma-separated string
        /// </summary>
        /// <param name="connection">SqliteConnection</param>
        /// <param name="tableName">Name of table in DB</param>
        /// <returns></returns>
        private static string GetColumnNamesList(SqliteConnection connection, string tableName)
        {
            if (!ColumnNames.ContainsKey(tableName))
            {
                ColumnNames.Add(tableName, string.Join(",", GetColumnNames(connection, tableName).ToArray()));
            }
            return ColumnNames[tableName];
        }

        /// <summary>
        /// Helper function to create a SQLite Database Connection
        /// </summary>
        /// <param name="dbPath">Path to the SQLite Database file</param>
        /// <param name="password">Password to Database</param>
        /// <returns></returns>
        public static SqliteConnection CreateDBConnection(string dbPath, string password)
        {
            // Create path if it doesn't exist
            string path = Path.GetDirectoryName(dbPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Check if db file exists
            if (!File.Exists(dbPath))
            {
                FileStream stream = File.Create(dbPath);
                stream.Close();
            }

            SqliteConnection connection = new SqliteConnection("DataSource=" + dbPath + ConnectionStrings.DB_Semicolon);
            connection.Open();
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT quote($password);";
                command.Parameters.AddWithValue("$password", password);
                string escapedPassword = (string)command.ExecuteScalar(); // Protects against SQL injection

                command.Parameters.Clear();
                command.CommandText = "PRAGMA key = " + escapedPassword + ConnectionStrings.DB_Semicolon;
                command.ExecuteNonQuery();
            }

            return connection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sourceTableName"></param>
        /// <param name="sourceColumnName"></param>
        /// <param name="sourceReferenceColumnName"></param>
        /// <param name="destinationTableName"></param>
        /// <param name="destinationColumnName"></param>
        /// <param name="destinationReferenceColumnName"></param>
        private static void MoveColumnInfo(SqliteConnection connection, string sourceTableName, string sourceColumnName, string sourceReferenceColumnName, string destinationTableName, string destinationColumnName, string destinationReferenceColumnName)
        {
            try
            {
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO " + destinationTableName + ConnectionStrings.DB_OpenParenthesis +
                        destinationColumnName + ConnectionStrings.DB_CloseParenthesis + " SELECT " + sourceColumnName + " FROM " +
                        sourceTableName + ConnectionStrings.DB_Semicolon;

                    command.ExecuteNonQuery();
                }
            }
            catch (System.Exception renameException)
            {
                Logger.Error(renameException, "DBI213");
                throw;
            }
        }

        /// <summary>
        /// Helper function to delete column from existing table
        /// </summary>
        /// <param name="connection">SqliteConnection</param>
        /// <param name="tableName">Name of table in DB</param>
        /// <param name="columnName">Name of column to delete</param>
        /// <returns></returns>
        private static bool DeleteColumn(SqliteConnection connection, string tableName, string columnName)
        {
            string colNames = string.Join(",", GetColumnNames(connection, tableName).ToArray());

            SqliteTransaction transaction = null;

            try
            {
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = ConnectionStrings.DB_Pragma_ForeignKeysOff;
                    command.ExecuteNonQuery();
                }

                transaction = connection.BeginTransaction();

                string createTableCmd = "";
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(ConnectionStrings.DB_SqliteMaster_Table, tableName);
                    SqliteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        createTableCmd = reader.GetString(0);
                    }
                }

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = ConnectionStrings.DB_AlterTable + tableName + " RENAME TO " +
                        tableName + "_old" + ConnectionStrings.DB_Semicolon;
                    command.ExecuteNonQuery();
                }

                int startIndex = createTableCmd.IndexOf(columnName);
                int endIndex = createTableCmd.IndexOf(ConnectionStrings.DB_Comma, startIndex) + 1;
                string substring = createTableCmd.Substring(startIndex, endIndex - startIndex);

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = createTableCmd.Replace(substring, "") + ConnectionStrings.DB_Semicolon;
                    command.ExecuteNonQuery();
                }

                using (SqliteCommand command = connection.CreateCommand())
                {
                    string oldValue = columnName + ",";

                    if (!colNames.Contains(oldValue))
                    {
                        oldValue = colNames.Contains("," + columnName) ? "," + columnName : columnName;
                    }

                    colNames = colNames.Replace(oldValue, "");

                    command.CommandText = "INSERT INTO " + tableName + ConnectionStrings.DB_OpenParenthesis +
                        colNames + ConnectionStrings.DB_CloseParenthesis + " SELECT " + colNames + " FROM " +
                        tableName + "_old" + ConnectionStrings.DB_Semicolon;

                    command.ExecuteNonQuery();
                }

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = "DROP TABLE " + tableName + "_old" + ConnectionStrings.DB_Semicolon;
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                transaction.Dispose();

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = ConnectionStrings.DB_Pragma_ForeignKeysOn;
                    command.ExecuteNonQuery();
                }

                return true;
            }
            catch (System.Exception renameException)
            {
                transaction?.Rollback();
                Logger.Error(renameException, "DBI212");
                throw;
            }
        }

        /// <summary>
        /// Helper function to add column to existing table
        /// </summary>
        /// <param name="connection">SqliteConnection</param>
        /// <param name="tableName">Name of table in DB</param>
        /// <param name="columnName">Name of column to add</param>
        /// <param name="columnDefinitions">Any column definitions, like type, constraints, etc.</param>
        /// <returns></returns>
        private static bool AddColumn(SqliteConnection connection, string tableName, string columnName, params string[] columnDefinitions)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                columnDefinitions.ToList().ForEach(s => sb.Append(" ").Append(s));

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = ConnectionStrings.DB_AlterTable + tableName +
                        ConnectionStrings.DB_AddColumn + columnName + sb.ToString() +
                        ConnectionStrings.DB_Semicolon;

                    int affectedRows = command.ExecuteNonQuery();

                    if (affectedRows >= 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (System.Exception columnAddException)
            {
                Logger.Error(columnAddException, "DBI011");
                return false;
            }
        }

        /// <summary>
        /// Helper function to rename an existing column in a table
        /// </summary>
        /// <param name="connection">SqliteConnection</param>
        /// <param name="tableName">Name of table in DB</param>
        /// <param name="columnName">Current name of column to rename</param>
        /// <param name="newColumnName">New name for column</param>
        /// <returns></returns>
        private static bool RenameColumn(SqliteConnection connection, string tableName, string columnName, string newColumnName)
        {
            string colNames = string.Join(",", GetColumnNames(connection, tableName).ToArray());

            SqliteTransaction transaction = null;

            try
            {
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = ConnectionStrings.DB_Pragma_ForeignKeysOff;
                    command.ExecuteNonQuery();
                }

                transaction = connection.BeginTransaction();

                string createTableCmd = "";
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(ConnectionStrings.DB_SqliteMaster_Table, tableName);
                    SqliteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        createTableCmd = reader.GetString(0);
                    }
                }

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = ConnectionStrings.DB_AlterTable + tableName + " RENAME TO " +
                        tableName + "_old" + ConnectionStrings.DB_Semicolon;
                    command.ExecuteNonQuery();
                }

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = createTableCmd.Replace(columnName + " ", newColumnName + " ") +
                        ConnectionStrings.DB_Semicolon;
                    command.ExecuteNonQuery();
                }

                using (SqliteCommand command = connection.CreateCommand())
                {
                    string oldValue = columnName + ",", newValue = newColumnName + ",";

                    if (!colNames.Contains(oldValue))
                    {
                        if (colNames.Contains("," + columnName))
                        {
                            oldValue = "," + columnName;
                            newValue = "," + newColumnName;
                        }
                        else
                        {
                            oldValue = columnName;
                            newValue = newColumnName;
                        }
                    }

                    command.CommandText = "INSERT INTO " + tableName + ConnectionStrings.DB_OpenParenthesis +
                        colNames.Replace(oldValue, newValue) + ConnectionStrings.DB_CloseParenthesis +
                        " SELECT " + colNames + " FROM " + tableName + "_old" + ConnectionStrings.DB_Semicolon;

                    command.ExecuteNonQuery();
                }

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = "DROP TABLE " + tableName + "_old" + ConnectionStrings.DB_Semicolon;
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                transaction.Dispose();

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = ConnectionStrings.DB_Pragma_ForeignKeysOn;
                    command.ExecuteNonQuery();
                }

                return true;
            }
            catch (System.Exception renameException)
            {
                transaction?.Rollback();
                Logger.Error(renameException, "DBI012");
                throw;
            }
        }

        /// <summary>
        /// Helper function to create foreign key statement
        /// </summary>
        /// <param name="tableName">Name of table in DB</param>
        /// <returns></returns>
        private static string References(string tableName)
        {
            return "REFERENCES " + tableName + "(Id)";
        }

        /// <summary>
        /// Helper function to create trigger on update for DateModified field
        /// </summary>
        /// <param name="connection">SqliteConnection</param>
        /// <param name="tableName">Name of table in DB</param>
        private static void CreateDateModifiedTrigger(SqliteConnection connection, string tableName)
        {
            try
            {
                // Drop Trigger, if exists
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = ConnectionStrings.DB_DropTrigger + ConnectionStrings.DB_IfExists +
                        tableName + ConnectionStrings.DB_Trigger_DateModified + ConnectionStrings.DB_Semicolon;
                    command.ExecuteNonQuery();
                }

                // Get current columns in table
                List<string> cols = GetColumnNames(connection, tableName);
                cols.Remove("Id");
                cols.Remove("DateCreated");
                cols.Remove("DateModified");

                if (cols.Count > 0)
                {
                    // Create Trigger
                    StringBuilder sb = new StringBuilder();
                    cols.ForEach(s => sb.Append(s).Append(ConnectionStrings.DB_Comma));
                    sb.Remove(sb.Length - 1, 1); // Remove last comma

                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = ConnectionStrings.DB_CreateTrigger + tableName +
                            ConnectionStrings.DB_Trigger_DateModified + ConnectionStrings.DB_Trigger_AfterUpdate +
                            sb.ToString() + string.Format(ConnectionStrings.DB_Trigger_UpdateStatement, tableName);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (System.Exception dateModifiedException)
            {
                Logger.Error(dateModifiedException, "DBI013");
            }
        }

        /// <summary>
        /// Helper function to ensure unique values in a table
        /// </summary>
        /// <param name="connection">SqliteConnection</param>
        /// <param name="tableName">Name of table in DB</param>
        /// <param name="uniqueColumns">Name(s) of column(s) (comma-separated if needed) required to be unique</param>
        private static void CreateUniqueIndex(SqliteConnection connection, string tableName, string uniqueColumns)
        {
            try
            {
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = ConnectionStrings.DB_CreateUniqueIndex + ConnectionStrings.DB_IfNotExists +
                        "Unique_" + tableName + " ON " + tableName + ConnectionStrings.DB_OpenParenthesis +
                        uniqueColumns +
                        ConnectionStrings.DB_CloseParenthesis + ConnectionStrings.DB_Semicolon;
                    command.ExecuteNonQuery();
                }
            }
            catch (System.Exception uniqueIndexException)
            {
                Logger.Error(uniqueIndexException, "DBI014");
            }
        }

        /// <summary>
        /// Helper function to insert values into tables
        /// </summary>
        /// <param name="connection">SqliteConnection</param>
        /// <param name="ignoreOnConflict">If true, use "INSERT OR IGNORE" (used to insert into column with UNIQUE constraint). If false, use "INSERT"</param>
        /// <param name="tableName">Name of table in DB</param>
        /// <param name="columnNames">Name(s) of column(s) (comma-separated if needed) being inserted into</param>
        /// <param name="values">Value(s) being inserted (comma-separated and 'single-quoted text' if needed)</param>
        private static List<string> Insert(SqliteConnection connection, bool ignoreOnConflict, string tableName, string columnNames, params string[] values)
        {
            List<string> ids = new List<string>();
            try
            {
                if (values.Length > 0 && values.ToList() is List<string> vals)
                {
                    //StringBuilder sb = new StringBuilder();
                    string guid = DataFunctions.GuidToSQLiteHexString(DataFunctions.NewGuidComb());
                    ids.Add(guid);

                    //vals.ForEach(s => sb.Append(ConnectionStrings.DB_OpenParenthesis + guid + ConnectionStrings.DB_Comma + s + ConnectionStrings.DB_CloseParenthesis + ConnectionStrings.DB_Comma));
                    //sb.Remove(sb.Length - 1, 1); // Remove last comma

                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        foreach (string item in vals)
                        {
                            // Creates a string that resembles => "(X'F67B93EE2412BF40B148AB67000000FC','1 - Critical')"
                            string valueToInsert = ConnectionStrings.DB_OpenParenthesis + guid + ConnectionStrings.DB_Comma + item + ConnectionStrings.DB_CloseParenthesis;

                            command.CommandText = $"INSERT {(ignoreOnConflict ? "OR IGNORE " : "")}INTO {tableName}{ConnectionStrings.DB_OpenParenthesis}Id,{columnNames}{ConnectionStrings.DB_CloseParenthesis} VALUES{valueToInsert}";
                            command.ExecuteNonQuery();

                            // Generating a new guid for this item.
                            guid = DataFunctions.GuidToSQLiteHexString(DataFunctions.NewGuidComb());
                        }

                        /// Former developer's method! Didn't work :(
                        /// This required n number of application startups to insert all content into the DB successfully, 
                        /// where n was the number of items that needed to be inserted into the database.
                        //command.CommandText = "INSERT " + (ignoreOnConflict ? "OR IGNORE " : "") + "INTO " + tableName +
                        //ConnectionStrings.DB_OpenParenthesis + "Id," + columnNames + ConnectionStrings.DB_CloseParenthesis +
                        //" VALUES" + sb.ToString();
                        //command.ExecuteNonQuery();
                    }
                }
            }
            catch (System.Exception insertException)
            {
                Logger.Error(insertException, "DBI024");
            }
            return ids;
        }

        #endregion

    }
}
