using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using Microsoft.Data.Sqlite;
using Serilog;
using TodoApp.Models.DatabaseModels;
using TodoApp.Models.DatabaseModels.Attributes;
#if ANDROID
using TodoApp.Platforms.Android;
#endif

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

        private static List<string> CreatedTables = new List<string>();
        public static List<T> GetData<T>() where T : BaseDataModel
        {
            try
            {
                CreateTableIfRequired<T>();
                return null;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #region Table Helpers
        private readonly static Func<PropertyInfo, bool> SearchFunctionIgnoringRequiredFields =
            x => x.GetCustomAttribute<DatabaseIgnoreAttribute>() == null;

        private static string GetTableName<T>() =>
            typeof(T).GetCustomAttribute<TableNameAttribute>() == null
            ? typeof(T).Name
            : typeof(T).GetCustomAttribute<TableNameAttribute>().TableName;

        private class ColumnNameList
        {
            public string Name { get; set; }
        }
        private static void CreateTableIfRequired<T>()
        {
            if (CreatedTables.Any(z => z == GetTableName<T>()))
            {
                return;
            }
            if (!GetDataFromSqlCommand<ColumnNameList>($"SELECT name FROM sqlite_master WHERE type='table' AND name='{GetTableName<T>()}';").Any())
            {
                ExecuteNonQuery(GetTableCreateStatement<T>());
            }
            else
            {
                var schema = GetCommand($"Select * from {GetTableName<T>()} LIMIT 1").ExecuteReader().GetColumnSchema();
                var missingColumns = ReflectionHelper.GetPropertyInfos(typeof(T), SearchFunctionIgnoringRequiredFields, BindingFlags.Public|BindingFlags.Instance)
                    .Where(x => !schema.Any(z =>
                        z.ColumnName.Equals(x.Name, StringComparison.CurrentCultureIgnoreCase))).ToList();
                if (missingColumns.Any())
                {
                    var sql = $"ALTER TABLE {GetTableName<T>()} ";
                    foreach (var missingColumn in missingColumns)
                    {
                        sql += $"ADD COLUMN {missingColumn.Name} {ConvertPropTypeToSqlLiteType(missingColumn)} ";
                    }

                    sql += ";";
                    if (!ExecuteNonQuery(sql))
                    {
#if ANDROID
                        MainActivity.NotificationHandler.SendNotification(MainActivity.NotificationHandler.GetDefaultNotificationBuilder($"Failed to update the database with new changes. Please clear the cache of this app and try again").Build(), TodoAppNotificationChannel.WarningNotificationId);
#endif
                    }
                }
            }

            CreatedTables.Add(GetTableName<T>());

        }

        private static string GetTableCreateStatement<T>()
        {
            var sql = $"CREATE TABLE IF NOT EXISTS {GetTableName<T>()} (";
            foreach (var prop in ReflectionHelper.GetPropertyInfos(typeof(T), SearchFunctionIgnoringRequiredFields, BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.Name.Equals("id", StringComparison.CurrentCultureIgnoreCase))
                {
                    sql += $"id INTEGER PRIMARY KEY AUTOINCREMENT,";
                    continue;
                }

                sql += $"{prop.Name} {ConvertPropTypeToSqlLiteType(prop)}, ";
            }

            return sql.TrimEnd(" ,".ToCharArray()) + ");";

        }

        private static string ConvertPropTypeToSqlLiteType(PropertyInfo prop)
        {
            //https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/types
            if (new List<Type>() { typeof(string), typeof(DateTime), typeof(TimeSpan) , typeof(decimal)}.Any(z => z == prop.PropertyType))
            {
                return "TEXT";
            }
            if (new List<Type>() { typeof(byte[]) }.Any(z => z == prop.PropertyType))
            {
                return "BLOB";
            }

            if (new List<Type>() { typeof(bool), typeof(byte), typeof(int), typeof(long) }.Any(z => z == prop.PropertyType))
            {
                return "INTEGER";
            }
            if (new List<Type>() { typeof(double)  }.Any(z => z == prop.PropertyType))
            {
                return "REAL";
            }

            throw new Exception($"Type is not a known one");

        }

        #endregion


        #region Sql Helpers
        private static List<T> GetDataFromSqlCommand<T>(string sqlToRun) where T : new()
        {
            Log.Warning($"Executing Query - {sqlToRun}");

            var reader = GetCommand(sqlToRun).ExecuteReader();
            var columnSchema = reader.GetColumnSchema();
            var returnVal = new List<T>();
            while (reader.Read())
            {
                var instance = new T();
                foreach (var column in columnSchema)
                {
                    var property = typeof(T).GetProperties().FirstOrDefault(x =>
                        x.Name.Equals(column.BaseColumnName, StringComparison.CurrentCultureIgnoreCase));
                    if (property is null)
                        continue;
                    var methodTyped = ReflectionHelper.GetMethodTyped(nameof(SqliteDataReader.GetFieldValue),
                        typeof(SqliteDataReader), property.PropertyType);
                    var valueToAdd = methodTyped.Invoke(reader, new object[]
                    {
                        column.ColumnOrdinal
                    });
                    property.SetValue(instance,valueToAdd );
                }
                returnVal.Add(instance );
            }

            return returnVal;
        }
        private static SqliteCommand GetCommand(string sqlToRun)
        {
            var command = DbConnection().CreateCommand();
            command.CommandText = sqlToRun;
            return command;
        }

        private static bool ExecuteNonQuery(string sqlToRun)
        {
            Log.Warning($"Executing SQL - {sqlToRun}");
            return GetCommand(sqlToRun).ExecuteNonQuery() == 1;
        }
        #endregion
    }
}
