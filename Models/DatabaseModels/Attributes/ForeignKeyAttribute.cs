namespace TodoApp.Models.DatabaseModels.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    class ForeignKeyAttribute : Attribute
    {
        public Type Type { get; set; }
        public ForeignKeyAttribute(Type type)
        {
            Type = type;
        }
    }
}
