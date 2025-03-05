using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using TodoApp.Models.DatabaseModels;
using TodoApp.Models.DatabaseModels.Attributes;

namespace TodoApp.Helpers
{
    class DatabaseHelper
    {
        #region Database initialization

        private static SqliteConnection _internalDbConnection { get; set; }

        private static SqliteConnection DbConnection()
        {
            _internalDbConnection ??= new SqliteConnection(new SqliteConnectionStringBuilder($"Data Source={Path.Combine(FileSystem.Current.CacheDirectory, "todo.db")}")
            {
                Mode = SqliteOpenMode.ReadWriteCreate,
            }.ConnectionString);
            _internalDbConnection.Open();
            return _internalDbConnection;
        }

        #endregion


        public static List<T> GetData<T>() where T : BaseDataModel
        {
            CreateTableIfRequired(GetTableName<T>());
            return null;
        }

        #region Table Helpers

        private static string GetTableName<T>() =>
            typeof(T).GetCustomAttribute<TableNameAttribute>() == null
            ? typeof(T).Name
            : typeof(T).GetCustomAttribute<TableNameAttribute>().TableName;


        private static void CreateTableIfRequired(string tableName)
        {
            if (!GetDataFromSqlCommand<string>($"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}';").Any())
            {
                //create table
            }
            //if table matches, but the columns dont, attempt to insert them
            //if we fail send a warning notification, and ask the user to clear cache

        }
        #endregion
        #region Sql Helpers
        private static List<T> GetDataFromSqlCommand<T>(string sqlToRun)
        {
            var reader = GetCommand(sqlToRun).ExecuteReader();

            var returnVal = new List<T>();
            foreach (var entry in reader)
            {

            }

            return returnVal;
        }
        private static SqliteCommand GetCommand(string sqlToRun)
        {
            var command = DbConnection().CreateCommand();
            command.CommandText = sqlToRun;
            return command;
        }


        #endregion
    }
}
