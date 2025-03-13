using System.Data.Common;
using System.Reflection;
using Microsoft.Data.Sqlite;
using Serilog;
using TodoApp.Models.DatabaseModels;
using TodoApp.Models.DatabaseModels.Attributes;
using TodoApp.Platforms.Android;

namespace TodoApp.Helpers
{
    class DatabaseHelper
    {
        #region Database initialization

        private static SqliteConnection _internalDbConnection { get; set; }

        private static SqliteConnection DbConnection()
        {
            _internalDbConnection ??= new SqliteConnection(
                new SqliteConnectionStringBuilder(
                    $"Data Source={Path.Combine(FileSystem.Current.CacheDirectory, "todo.db")}")
                {
                    Mode = SqliteOpenMode.ReadWriteCreate,
                }.ConnectionString);
            _internalDbConnection.Open();
            return _internalDbConnection;
        }

        private static readonly string DefaultWhereClause = "active=true";

        #endregion


        public static List<T> GetData<T>(params string[] whereClause) where T : BaseDataModel, new()
        {
            CreateTableIfRequired<T>();

            return GetDataFromSqlCommand<T>($"{GetSelectQuery<T>()} where {string.Join(",", whereClause)}");
        }

        public static List<T> GetData<T>() where T : BaseDataModel, new() => GetData<T>("true");

        public static List<T?> GetData<T>(params long[] id) where T : BaseDataModel, new()
        {
            CreateTableIfRequired<T>();
            return id.Select(x =>
                    GetDataFromSqlCommand<T>(
                        $"{GetSelectQuery<T>()} where {SqlHelpers.EqualToo(nameof(BaseDataModel.Id), x)}")
                    .FirstOrDefault())
                .ToList();
        }

        public static T? SelectData<T>(string whereClause) where T : BaseDataModel, new()
            => GetData<T>(whereClause).FirstOrDefault();

        public static T? SelectData<T>(long id) where T : BaseDataModel, new()
            => GetData<T>(id).FirstOrDefault();


        public static void UpsertData<T>(params T[] data) where T : BaseDataModel, new()
        {
            CreateTableIfRequired<T>();
            foreach (var record in data)
            {
                if (record.Id <= 0)
                    ExecuteNonQuery(GetCreateStatement(record));
                else
                    ExecuteNonQuery(GetUpdateStatement(record));

            }
        }

        public static List<T?> UpsertDataWithReturn<T>(params T[] data) where T : BaseDataModel, new()
        {
            var returnList = new List<T?>();
            foreach (var instance in data)
            {
                var id = instance.Id;
                UpsertData(data);
                if (instance.Id <= 0)
                {
                    var queryResult = GetCommand("SELECT last_insert_rowid()").ExecuteReader();
                    queryResult.Read();
                    id = queryResult.GetInt64(0);
                }

                returnList.AddRange(GetData<T>(id));
            }

            return returnList;
        }


        #region Table Helpers

        private static List<string> EnrichWhereClause(string[] whereStrings)
        {
            var returnVal = new List<string>(whereStrings);
            returnVal.Add(DefaultWhereClause);
            return returnVal;
        }

        private static List<PropertyInfo> GetRelevantProperties(Type type) => ReflectionHelper.GetPropertyInfos(type,
                x => x.GetCustomAttribute<DatabaseIgnoreAttribute>() == null &&
                     x.GetCustomAttribute<AutoPopulateAttribute>() == null, BindingFlags.Public | BindingFlags.Instance)
            .ToList();

        private static string GetTableName(Type type) =>
            type.GetCustomAttribute<TableNameAttribute>() == null
                ? type.Name
                : type.GetCustomAttribute<TableNameAttribute>().TableName;

        private static string GetTableName<T>() => GetTableName(typeof(T));

        private class ColumnNameList
        {
            public string Name { get; set; }
        }

        private static List<string> CreatedTables = new List<string>();


        private static void CreateTableIfRequired<T>()
        {
            if (CreatedTables.Any(z => z == GetTableName<T>()))
            {
                return;
            }

            if (!GetDataFromSqlCommand<ColumnNameList>(
                    $"SELECT name FROM sqlite_master WHERE type='table' AND name='{GetTableName<T>()}';").Any())
            {
                ExecuteNonQuery(GetTableCreateStatement<T>());
            }
            else
            {
                var schema = GetCommand($"Select * from {GetTableName<T>()} LIMIT 1").ExecuteReader().GetColumnSchema();
                var missingColumns = GetRelevantProperties(typeof(T)).Where(x => !schema.Any(z =>
                    z.ColumnName.Equals(x.Name, StringComparison.CurrentCultureIgnoreCase))).ToList();
                if (missingColumns.Any())
                {
                    var sql = $"ALTER TABLE {GetTableName<T>()} ";
                    foreach (var missingColumn in missingColumns)
                    {
                        sql += $"ADD COLUMN {missingColumn.Name} {ConvertPropTypeToSqlLiteType(missingColumn)} ";
                    }

                    sql += ";";
                    var result = ExecuteNonQuery(sql);
                    if (!ExecuteNonQuery(sql))
                    {
                        MainActivity.NotificationHandler.SendNotification(
                            MainActivity.NotificationHandler
                                .GetDefaultNotificationBuilder(
                                    $"Failed to update the database with new changes. Please clear the cache of this app and try again")
                                .Build(), TodoAppNotificationChannel.WarningNotificationId);
                    }
                }
            }

            CreatedTables.Add(GetTableName<T>());

        }

        private static string GetTableCreateStatement<T>()
        {
            var sql = $"CREATE TABLE IF NOT EXISTS {GetTableName<T>()} (";
            var fields = GetRelevantProperties(typeof(T));
            foreach (var prop in fields)
            {
                if (prop.Name.Equals("id", StringComparison.CurrentCultureIgnoreCase))
                {
                    sql += $"id INTEGER PRIMARY KEY AUTOINCREMENT,";
                    continue;
                }

                //this is a foreign key, setup the constraint 
                if (prop.GetCustomAttribute<ForeignKeyAttribute>() != null)
                {
                    sql += $"{prop.Name} {ConvertPropTypeToSqlLiteType(prop)} , ";
                    continue;
                }

                sql += $"{prop.Name} {ConvertPropTypeToSqlLiteType(prop)}, ";
            }

            foreach (var field in fields.Where(x => x.GetCustomAttribute<ForeignKeyAttribute>() != null))
            {
                var attribute = field.GetCustomAttribute<ForeignKeyAttribute>();
                sql += $"FOREIGN KEY ({field.Name}) " +
                       $"REFERENCES {GetTableName(attribute.Type)} (id)";
            }

            sql = sql.TrimEnd(", ".ToCharArray());

            return sql + ");";

        }

        private static string ConvertPropTypeToSqlLiteType(PropertyInfo prop)
        {
            //https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/types
            if (new List<Type>() { typeof(string), typeof(DateTime), typeof(TimeSpan), typeof(decimal), typeof(DateTime?) }.Any(z =>
                    z == prop.PropertyType))
            {
                return "TEXT nocase";
            }

            if (new List<Type>() { typeof(byte[]) }.Any(z => z == prop.PropertyType))
            {
                return "BLOB";
            }

            if (new List<Type>() { typeof(bool), typeof(byte), typeof(int), typeof(long) }.Any(z =>
                    z == prop.PropertyType || prop.PropertyType.IsEnum))
            {
                return "INTEGER";
            }

            if (new List<Type>() { typeof(double) }.Any(z => z == prop.PropertyType))
            {
                return "REAL";
            }

            throw new Exception($"Type is not a known one");

        }



        #endregion


        #region Sql Helpers

        private static List<PropertyInfo> GetCreateUpdateApplicableProperties<T>() =>
            typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => !x.Name.Equals("id", StringComparison.CurrentCultureIgnoreCase) &&
                            x.GetCustomAttribute<AutoPopulateAttribute>() == null && x.SetMethod != null).ToList();

        private static string GetUpdateStatement<T>(T instance) where T : BaseDataModel, new()
            => $"update {GetTableName<T>()} set " +
               string.Join(',',
                   GetCreateUpdateApplicableProperties<T>().Select(x =>
                       $"{x.Name} = {SqlHelpers.GetStringInCorrectWrappers(GetColumnValue(x.GetValue(instance)), x.PropertyType)}"))
               + $" where {SqlHelpers.EqualToo(nameof(BaseDataModel.Id), instance.Id)}";

        private static string GetCreateStatement<T>(T instance)
            => $"insert into {GetTableName<T>()} " +
               $"({string.Join(',', GetCreateUpdateApplicableProperties<T>().Select(x => $"{x.Name}"))})" +
               $" values ({string.Join(',', GetCreateUpdateApplicableProperties<T>().Select(x => SqlHelpers.GetStringInCorrectWrappers(GetColumnValue(x.GetValue(instance)), x.PropertyType)))})";

        private static string GetColumnValue(object? instance)
        {
            if (instance == null) return "";
            if (instance.GetType().IsEnum)
            {
                return "" + ((int)instance);
            }

            return instance.ToString();
        }

        private static string GetSelectQuery<T>()
        {
            var returnVal = $"select ";
            returnVal += string.Join(',', typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.GetCustomAttribute<AutoPopulateAttribute>() == null && x.SetMethod != null)
                .Select(x => $"{x.Name}"));
            return returnVal + " from " + GetTableName<T>();
        }

        private static List<T> GetDataFromSqlCommand<T>(string sqlToRun) where T : new()
        {
            try
            {


                Log.Warning($"Executing Query - {sqlToRun}");

                var reader = GetCommand(sqlToRun).ExecuteReader();
                var columnSchema = reader.GetColumnSchema();
                var returnVal = new List<T>();
                while (reader.Read())
                {
                    var instance = new T();
                    foreach (var column in columnSchema.Where(x => x.ColumnOrdinal.HasValue))
                    {
                        var property = typeof(T).GetProperties().FirstOrDefault(x =>
                            x.Name.Equals(column.BaseColumnName, StringComparison.CurrentCultureIgnoreCase));
                        if (property is null)
                            continue;
                        if (reader.IsDBNull(column.ColumnOrdinal.Value) ||
                            (reader.GetFieldType(column.ColumnOrdinal.Value) == typeof(string) &&
                             reader.GetFieldValue<string>(column.ColumnOrdinal.Value) == ""))
                            continue;

                        var methodTyped = ReflectionHelper.GetMethodTyped(nameof(SqliteDataReader.GetFieldValue),
                            typeof(SqliteDataReader), property.PropertyType);
                        var valueToAdd = methodTyped.Invoke(reader, new object[]
                        {
                            column.ColumnOrdinal.Value
                        });
                        if (property.GetSetMethod() != null && valueToAdd != null)
                        {

                            property.SetValue(instance, valueToAdd);

                        }

                    }


                    returnVal.Add(instance);
                }

                var foreignKeyProperties = GetRelevantProperties(typeof(T))
                    .Where(x => x.GetCustomAttribute<ForeignKeyAttribute>() != null);
                //foreach table we're linked with 
                foreach (var foreignKey in foreignKeyProperties)
                {

                    //get all the properties should be auto populated on this foreign key
                    foreach (var property in typeof(T).GetProperties(ReflectionHelper.DefaultBindingFlags).Where(x =>
                                 x.GetCustomAttribute<AutoPopulateAttribute>() != null &&
                                 x.PropertyType ==
                                 foreignKey.GetCustomAttribute<ForeignKeyAttribute>().Type))
                    {
                        foreach (var instance in returnVal.Where(x => x != null))
                        {
                            var idToSearchFor = long.Parse(foreignKey.GetValue(instance).ToString());
                            var typedMethodInstance = ReflectionHelper.GetMethodTyped(nameof(DatabaseHelper.SelectData),
                                typeof(DatabaseHelper), property.PropertyType, new[] { typeof(long) });
                            property.SetValue(instance, typedMethodInstance.Invoke(null, new object[]
                            {
                                idToSearchFor
                            }));
                        }
                    }
                }


                return returnVal;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private static SqliteCommand GetCommand(string sqlToRun)
        {
            var command = DbConnection().CreateCommand();
            command.CommandText = sqlToRun;
            return command;
        }

        private static bool ExecuteNonQuery(string sqlToRun)
        {
            try
            {
                Log.Warning($"Executing SQL - {sqlToRun}");
                var result = GetCommand(sqlToRun).ExecuteNonQuery() == 1;
                return result;

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
    }
}
