namespace TodoApp.Models.DatabaseModels.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class TableNameAttribute : Attribute
    {
        public string TableName { get; set; }
        public TableNameAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}
