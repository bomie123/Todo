namespace TodoApp.Models.DatabaseModels.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    class DatabaseIgnoreAttribute : Attribute
    {
        public DatabaseIgnoreAttribute(){}
    }
}
