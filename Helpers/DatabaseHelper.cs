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

        private static SqliteConnection DbConnection { get;  } //= new SqliteConnection(Path.Combine(FileSystem.AppDataDirectory, "todoSql.db3"));
      

        #endregion
        public static List<T> GetData<T>() where T : BaseDataModel
        {
            var connection = new SqliteConnection(Path.Combine(FileSystem.AppDataDirectory, "todoSql.db3"));
            CreateTableIfRequired(GetTableName<T>());
            return null;
            DbConnection.CreateCommand().ExecuteNonQuery();
        }

        #region Helpers

        private static string GetTableName<T>() => 
            typeof(T).GetCustomAttribute<TableNameAttribute>() != null
            ? typeof(T).Name
            : typeof(T).GetCustomAttribute<TableNameAttribute>().TableName;
        private static void CreateTableIfRequired(string tableName)
        {
            if (GetDataFromSqlCommand<string>($"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}';").Any())
            {

            }
        }

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
            var command = DbConnection.CreateCommand();
            command.CommandText = sqlToRun;
            return command;
        }

  
        #endregion
    }
}
